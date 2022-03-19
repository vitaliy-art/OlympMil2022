package models

import (
	"context"
	"fmt"
	"os/exec"
	"strings"
	"sync"
	"time"
)

type Action struct {
	duration time.Duration
	result   []string
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

	<-time.After(5 * time.Second)
	cancel()
	wg.Wait()

	if a.duration < 0 {
		a.duration = 0
	}

	if err != nil {
		a.result = strings.Split(err.Error(), "\n")
	} else {
		a.result = strings.Split(string(out), "\n")
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
	result += fmt.Sprint(strings.Repeat(" ", 8), "Success: ", a.IsSuccess(), "\n")
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
