package main

import (
	"checker/src/checker/cmd/checker/config"
	"checker/src/checker/cmd/checker/files"
	"checker/src/checker/internal"
	"checker/src/checker/models"
	"context"
	"fmt"
	"os"
	"strings"
)

var totalScore = 0.0
var colorReset = "\033[0m"
var colorRed = "\033[31m"
var colorGreen = "\033[32m"

func main() {
	config := config.GetConfig()
	params := os.Args[1:]

	filenames := files.GetFilenames()
	stages := files.GetStages(filenames)
	stagesForSend := []models.Stage{}

	for i := 1; i < len(stages)+1; i++ {
		stage := stages[int32(i)]
		stage.Run(params)
		stagesForSend = append(stagesForSend, *stage)

		if stages[int32(i)].GetSuccess() {
			fmt.Print(string(colorGreen), "Test ", i, ": Ok\n")
		} else {
			fmt.Print(string(colorRed), "Test ", i, ": Fail\n")
		}
	}

	client := internal.GetDefaultLocalGrpcClient()
	_ = client.SaveStages(context.Background(), config.CheckerId, config.IsFinal, stagesForSend)

	fmt.Print(string(colorReset), "\n\n")
	failResults := ""
	successResults := ""
	successCount := 0

	for i := 1; i < len(stages)+1; i++ {
		result := stages[int32(i)].GetResultsString() + "\n\n" + strings.Repeat("=", 24) + "\n" + strings.Repeat("=", 24) + "\n\n"

		if stages[int32(i)].GetSuccess() {
			totalScore += stages[int32(i)].Score
			successResults += result
			successCount += 1
		} else {
			failResults += result
		}
	}

	fmt.Println("Files with results:")

	if failResults != "" {
		fmt.Println(files.WriteFail(failResults))
	}

	if successResults != "" {
		fmt.Println(files.WriteSuccess(successResults))
	}

	fmt.Println()
	fmt.Println()
	fmt.Println("Result ", successCount, "/", len(stages))
	fmt.Println("Score: ", totalScore)
}
