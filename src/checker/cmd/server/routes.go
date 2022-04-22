package main

import (
	"checker/src/checker/cmd/server/controllers"

	"github.com/gin-gonic/gin"
)

func setRoutes(r *gin.Engine) {
	r.GET("", controllers.Index)
	r.GET("index", controllers.Index)
	r.GET(":recordId", controllers.GetRecordById)
}
