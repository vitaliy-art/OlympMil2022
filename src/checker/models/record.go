package models

import (
	"encoding/json"

	"gorm.io/gorm"
)

type Record struct {
	*gorm.Model
	Uuid   string `gorm:"index"`
	Stages []byte
}

func (r *Record) SetStages(s []Stage) (err error) {
	r.Stages, err = json.Marshal(s)
	return
}

func (r Record) GetStages() (stages []Stage, err error) {
	err = json.Unmarshal(r.Stages, &stages)
	return
}
