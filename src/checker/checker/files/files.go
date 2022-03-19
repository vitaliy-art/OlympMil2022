package files

import (
	"checker/src/checker/checker/models"
	"encoding/json"
	"fmt"
	"io/ioutil"
	"log"
	"os"
	"path/filepath"
	"strconv"
	"strings"
	"time"
)

var filenames = []string{}

func init() {
	fileInfos, err := ioutil.ReadDir("./stages/")

	if err != nil {
		log.Fatal(err)
	}

	for _, info := range fileInfos {
		if !info.IsDir() {
			if strings.HasPrefix(info.Name(), "test_stage_") && strings.HasSuffix(info.Name(), ".json") {
				filenames = append(filenames, "./stages/"+info.Name())
			}
		}
	}
}

func GetFilenames() []string {
	return filenames
}

func GetStages(filenames []string) map[int32]*models.Stage {
	stages := map[int32]*models.Stage{}

	for _, fn := range filenames {
		fnId := strings.ReplaceAll(strings.ReplaceAll(fn, ".json", ""), "./stages/test_stage_", "")
		id, err := strconv.ParseInt(fnId, 10, 32)

		if err != nil {
			log.Fatal(err)
		}

		stage := &models.Stage{}
		plan, err := ioutil.ReadFile(fn)

		if err != nil {
			log.Fatal(err)
		}

		if err = json.Unmarshal(plan, stage); err != nil {
			log.Fatal(err)
		}

		stage.SetId(int32(id))
		stages[int32(id)] = stage
	}

	return stages
}

func writeResults(result, dir string) string {
	path := filepath.Join(".", dir)
	fName := filepath.Join(path, fmt.Sprint(time.Now().Format("2006-01-02 15-04-05.99"), ".txt"))

	if err := os.MkdirAll(path, os.ModePerm); err != nil {
		panic(err)
	}

	if err := ioutil.WriteFile(fName, []byte(result), os.ModeExclusive); err != nil {
		panic(err)
	}

	return fName
}

func WriteFail(s string) string {
	return writeResults(s, "fails")
}

func WriteSuccess(s string) string {
	return writeResults(s, "successes")
}
