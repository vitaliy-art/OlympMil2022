package main

import (
	"checker/src/checker/checker/files"
	"fmt"
	"os"
	"strings"
)

var totalScore = 0.0
var colorReset = "\033[0m"
var colorRed = "\033[31m"
var colorGreen = "\033[32m"

func main() {
	params := os.Args[1:]

	filenames := files.GetFilenames()
	stages := files.GetStages(filenames)

	for i := 1; i < len(stages)+1; i++ {
		stages[int32(i)].Run(params)

		if stages[int32(i)].GetSuccess() {
			fmt.Print(string(colorGreen), i)
		} else {
			fmt.Print(string(colorRed), i)
		}
	}

	fmt.Print(string(colorReset), "\n\n")
	failResults := ""
	successResults := ""

	for _, s := range stages {
		result := s.GetResultsString() + "\n\n" + strings.Repeat("=", 24) + "\n" + strings.Repeat("=", 24)

		if s.GetSuccess() {
			totalScore += s.Score
			successResults += result
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
	fmt.Println("Result: ", totalScore)
}
