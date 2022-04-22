package main

import (
	generated "checker/src/checker/cmd/server/grpc"
	"checker/src/checker/cmd/server/grpcServer"
	"net"

	"github.com/gin-gonic/gin"
	"google.golang.org/grpc"
	"google.golang.org/grpc/reflection"
)

var httpFinish = make(chan bool)
var grpcFinish = make(chan bool)

func main() {
	go runHttpServer()
	go runGrpcServer()

	switch {
	case <-httpFinish:
	case <-grpcFinish:
	}
}

func runHttpServer() {
	r := gin.New()
	r.Use(gin.Logger())
	r.Use(gin.Recovery())
	r.LoadHTMLGlob("./templates/*.tmpl.html")
	r.Static("/static", "./templates/static/")
	setRoutes(r)

	if err := r.Run("0.0.0.0:8181"); err != nil {
		panic(err)
	}

	close(httpFinish)
}

func runGrpcServer() {
	lis, err := net.Listen("tcp", ":8182")

	if err != nil {
		panic(err)
	}

	s := grpc.NewServer()
	generated.RegisterStagesServiceServer(s, &grpcServer.StagesServiceServerImpl{})
	reflection.Register(s)

	if err := s.Serve(lis); err != nil {
		panic(err)
	}

	close(grpcFinish)
}
