package models

import (
	"checker/src/checker/cmd/server/grpc"
	"fmt"
	"strings"
	"time"

	"google.golang.org/protobuf/types/known/durationpb"
)

type Stage struct {
	duration time.Duration
	success  bool
	id       int32

	Actions     []*Action `json:"actions"`
	Score       float64   `json:"score"`
	Description string    `json:"description"`
}

func (s *Stage) SetDuration(d time.Duration) {
	s.duration = d
}

func (s Stage) GetDuration() time.Duration {
	return s.duration
}

func (s *Stage) SetSuccess(suc bool) {
	s.success = suc
}

func (s Stage) GetSuccess() bool {
	return s.success
}

func (s *Stage) SetId(i int32) {
	s.id = i
}

func (s Stage) GetId() int32 {
	return s.id
}

func (s *Stage) Run(params []string) {
	s.success = true

	for _, a := range s.Actions {
		a.Run(params)
		s.duration += a.duration

		if a.IsCritical && !a.IsSuccess() {
			s.success = false
		}
	}
}

func (s Stage) GetResultsString() string {
	result := ""
	result += fmt.Sprint("Stage ", s.id, "\n")
	result += fmt.Sprint("Description: ", s.Description, "\n")
	result += fmt.Sprint(strings.Repeat(" ", 4), "Success: ", s.success, "\n")
	result += fmt.Sprint(strings.Repeat(" ", 4), "Duration: ", s.duration, "\n")

	for _, a := range s.Actions {
		result += a.GetResultString()
	}

	return result
}

func (s *Stage) FromProto(pStage *grpc.Stage) {
	s.duration = pStage.Duration.AsDuration()
	s.id = pStage.Id
	s.success = pStage.Success
	s.Actions = []*Action{}

	for _, a := range pStage.Actions {
		action := &Action{}
		action.FromProto(a)
		s.Actions = append(s.Actions, action)
	}

	s.Description = pStage.Description
	s.Score = float64(pStage.Score)
}

func (s Stage) ToProto() *grpc.Stage {
	actions := []*grpc.Action{}

	for _, a := range s.Actions {
		action := a.ToProto()
		actions = append(actions, action)
	}

	return &grpc.Stage{
		Id:          s.id,
		Score:       float32(s.Score),
		Description: s.Description,
		Duration:    durationpb.New(s.duration),
		Success:     s.success,
		Actions:     actions,
	}
}
