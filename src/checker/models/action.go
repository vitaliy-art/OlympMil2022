package models

import (
	"checker/src/checker/cmd/server/grpc"
	"context"
	"fmt"
	"os/exec"
	"strings"
	"sync"
	"time"

	"google.golang.org/protobuf/types/known/durationpb"
)

type Action struct {
	duration time.Duration
	result   []string
	err      []string
	command  []string

	Id         int32    `json:"id"`
	Parameters []string `json:"parameters"`
	Expected   []string `json:"expected"`
	IsCritical bool     `json:"is_critical"`
}

func (a Action) GetDuration() time.Duration {
	return a.duration
}

func (a Action) GetResult() []string {
	return a.result
}

func (a *Action) Run(params []string) {
	name := params[0]
	a.command = append(params, a.Parameters...)
	params = append(params[1:], a.Parameters...)
	var wg sync.WaitGroup
	ctx, cancel := context.WithCancel(context.Background())
	cmd := exec.CommandContext(ctx, name, params...)
	var out []byte
	var err error

	wg.Add(1)
	go func() {
		now := time.Now()
		out, err = cmd.Output()
		a.duration = time.Since(now)
		wg.Done()
	}()

	for i := 0; i < 5; i++ {
		<-time.After(time.Second)

		if out != nil || err != nil {
			break
		}
	}

	if a.duration == 0 {
		a.duration = time.Second * 5
	}

	cancel()
	wg.Wait()

	if a.duration < 0 {
		a.duration = 0
	}

	if err != nil {
		a.err = strings.Split(err.Error(), "\n")
	}

	a.result = strings.Split(string(out), "\n")

	i := len(a.result) - 1

	if a.result[i] == "" && i != 0 {
		a.result = a.result[:i]
	}
}

func (a Action) IsSuccess() (result bool) {
	if len(a.result) != len(a.Expected) {
		return
	}

	for i, value := range a.result {
		if a.Expected[i] != value {
			return
		}
	}

	return true
}

func (a Action) GetResultString() string {
	result := fmt.Sprint(strings.Repeat(" ", 4), "Action ", a.Id, "\n")

	if a.err != nil {
		result += fmt.Sprint(strings.Repeat(" ", 8), "Error:\n")

		for _, e := range a.err {
			result += fmt.Sprint(strings.Repeat(" ", 12), e, "\n")
		}
	}

	result += fmt.Sprint(strings.Repeat(" ", 8), "Success: ", a.IsSuccess(), "\n")
	result += fmt.Sprint(strings.Repeat(" ", 8), "Duration: ", a.duration, "\n")
	result += fmt.Sprint(strings.Repeat(" ", 8), "Critical: ", a.IsCritical, "\n")
	result += fmt.Sprint(strings.Repeat(" ", 8), "Command: ")

	for _, p := range a.command {
		result += p + " "
	}

	result += "\n"
	result += fmt.Sprint(strings.Repeat(" ", 8), "Expected:\n")

	for _, e := range a.Expected {
		result += fmt.Sprint(strings.Repeat(" ", 12), e, "\n")
	}

	result += fmt.Sprint(strings.Repeat(" ", 8), "Result:\n")

	for _, r := range a.result {
		result += fmt.Sprint(strings.Repeat(" ", 12), r, "\n")
	}

	result += "\n"
	return result
}

func (a *Action) FromProto(pAction *grpc.Action) {
	a.command = pAction.Command
	a.duration = pAction.Duration.AsDuration()
	a.err = pAction.Error
	a.result = pAction.Result
	a.Expected = pAction.Expected
	a.Id = pAction.Id
	a.IsCritical = pAction.IsCritical
	a.Parameters = pAction.Parameters
}

func (a Action) ToProto() *grpc.Action {
	return &grpc.Action{
		Id:         a.Id,
		Parameters: a.Parameters,
		IsCritical: a.IsCritical,
		Duration:   durationpb.New(a.duration),
		Expected:   a.Expected,
		Result:     a.result,
		Error:      a.err,
		Command:    a.command,
	}
}
