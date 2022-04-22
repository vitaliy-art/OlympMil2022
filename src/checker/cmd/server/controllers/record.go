package controllers

import (
	"checker/src/checker/cmd/server/utils"
	"checker/src/checker/models"
	"net/http"
	"strings"

	"github.com/gin-gonic/gin"
	"gorm.io/gorm"
)

func GetRecordById(c *gin.Context) {
	id := c.Param("recordId")

	if id == "" {
		c.Redirect(http.StatusSeeOther, "/")
		return
	}

	db := utils.GetDB()
	record := &models.Record{}

	if err := db.Find(record, &models.Record{Uuid: id}).Error; err == gorm.ErrRecordNotFound {
		c.AbortWithError(http.StatusNotFound, err)
		return
	}

	stages, err := record.GetStages()

	if err != nil {
		c.AbortWithError(http.StatusInternalServerError, err)
		return
	}

	type StageResult struct {
		Result []string
	}

	fails := []StageResult{}
	successes := []StageResult{}

	for _, stage := range stages {
		if stage.Success {
			successes = append(successes, StageResult{Result: strings.Split(stage.GetResultsString(), "\n")})
		} else {
			fails = append(fails, StageResult{Result: strings.Split(stage.GetResultsString(), "\n")})
		}
	}

	c.HTML(http.StatusOK, "record.tmpl.html", gin.H{
		"fails":        fails,
		"successes":    successes,
		"is_fails":     len(fails) > 0,
		"is_successes": len(successes) > 0,
	})
}
