package config

import (
	"encoding/json"
	"io/fs"
	"io/ioutil"
	"log"

	"github.com/google/uuid"
)

type Config struct {
	CheckerId     string `json:"checkerId"`
	StagesDirName string `json:"stagesDirName"`
	CheckerHost   string `json:"checkerHost"`
	CheckerPort   int32  `json:"checkerPort"`
	IsFinal       bool   `json:"final"`
}

var config = &Config{}

func init() {
	plan, err := ioutil.ReadFile("./config.json")

	if err != nil {
		log.Println(err.Error())
		return
	}

	if err := json.Unmarshal(plan, config); err != nil {
		log.Println(err.Error())
		return
	}

	if config.CheckerId == "" {
		config.CheckerId = uuid.New().String()
		str, err := json.Marshal(config)

		if err != nil {
			log.Println(err.Error())
			config.CheckerId = ""
			return
		}

		if err := ioutil.WriteFile("./config.json", []byte(str), fs.ModePerm); err != nil {
			config.CheckerId = ""
			log.Println(err.Error())
			return
		}
	}
}

func GetConfig() Config {
	return *config
}
