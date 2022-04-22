package models

import (
	"checker/src/checker/cmd/server/grpc"
	"fmt"
	"strings"
	"time"

	"google.golang.org/protobuf/types/known/durationpb"
)

type Stage struct {
	Duration time.Duration
	Success  bool
	Id       int32

	Actions     []*Action `json:"actions"`
	Score       float64   `json:"score"`
	Description string    `json:"description"`
}

func (s *Stage) SetDuration(d time.Duration) {
	s.Duration = d
}

func (s Stage) GetDuration() time.Duration {
	return s.Duration
}

func (s *Stage) SetSuccess(suc bool) {
	s.Success = suc
}

func (s Stage) GetSuccess() bool {
	return s.Success
}

func (s *Stage) SetId(i int32) {
	s.Id = i
}

func (s Stage) GetId() int32 {
	return s.Id
}

func (s *Stage) Run(params []string) {
	s.Success = true

	for _, a := range s.Actions {
		a.Run(params)
		s.Duration += a.Duration

		if a.IsCritical && !a.IsSuccess() {
			s.Success = false
		}
	}
}

func (s Stage) GetResultsString() string {
	result := ""
	result += fmt.Sprint("Stage ", s.Id, "\n")
	result += fmt.Sprint("Description: ", s.Description, "\n")
	result += fmt.Sprint(strings.Repeat(" ", 4), "Success: ", s.Success, "\n")
	result += fmt.Sprint(strings.Repeat(" ", 4), "Duration: ", s.Duration, "\n")

	for _, a := range s.Actions {
		result += a.GetResultString()
	}

	return result
}

func (s *Stage) FromProto(pStage *grpc.Stage) {
	s.Duration = pStage.Duration.AsDuration()
	s.Id = pStage.Id
	s.Success = pStage.Success
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
		Id:          s.Id,
		Score:       float32(s.Score),
		Description: s.Description,
		Duration:    durationpb.New(s.Duration),
		Success:     s.Success,
		Actions:     actions,
	}
}
