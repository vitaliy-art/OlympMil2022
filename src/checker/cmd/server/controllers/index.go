package controllers

import (
	"checker/src/checker/cmd/server/utils"
	"checker/src/checker/models"
	"net/http"

	"github.com/gin-gonic/gin"
)

func Index(c *gin.Context) {
	db := utils.GetDB()
	records := []models.Record{}
	result := db.Order("updated_at desc").Find(&records)

	if err := result.Error; err != nil {
		c.AbortWithError(http.StatusInternalServerError, err)
		return
	}

	type Result struct {
		SuccessStages int32
		StagesCount   int32
		Score         float32
	}

	results := map[string]Result{}

	for _, r := range records {
		stages, err := r.GetStages()

		if err != nil {
			c.AbortWithError(http.StatusInternalServerError, err)
			return
		}

		result := &Result{
			StagesCount: int32(len(stages)),
		}

		for _, s := range stages {
			if s.GetSuccess() {
				result.Score += float32(s.Score)
				result.SuccessStages += 1
			}
		}

		results[r.Uuid] = *result
	}

	c.HTML(http.StatusOK, "index.tmpl.html", gin.H{
		"results": results,
	})
}
