package main

import "github.com/gin-gonic/gin"

func main() {
	r := gin.New()
	r.Use(gin.Logger())
	r.Use(gin.Recovery())
	r.LoadHTMLGlob("./templates/*.tmpl.html")
	r.Static("/static", "./templates/static/")
	setRoutes(r)
	r.Run("0.0.0.0:8181")
}
