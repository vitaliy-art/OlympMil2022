package models

import (
	"fmt"
	"strings"
	"time"
)

type Stage struct {
	duration time.Duration
	success  bool
	id       int32

	Actions []*Action `json:"actions"`
	Score   float64   `json:"score"`
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

		if s.id == 14 {
			fmt.Println()
		}

		if a.IsCritical && !a.IsSuccess() {
			s.success = false
		}
	}
}

func (s Stage) GetResultsString() string {
	result := ""
	result += fmt.Sprint("Stage ", s.id, "\n")
	result += fmt.Sprint(strings.Repeat(" ", 4), "Success: ", s.success, "\n")
	result += fmt.Sprint(strings.Repeat(" ", 4), "Duration: ", s.duration, "\n")

	for _, a := range s.Actions {
		result += a.GetResultString()
	}

	return result
}
