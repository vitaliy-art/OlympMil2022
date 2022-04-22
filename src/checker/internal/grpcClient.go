package internal

import (
	generated "checker/src/checker/cmd/server/grpc"
	"checker/src/checker/models"
	"context"
	"fmt"

	"google.golang.org/grpc"
	"google.golang.org/grpc/credentials/insecure"
)

type GrpcClient struct {
	Cfg GrpcClientConfig
}

type GrpcClientConfig struct {
	Host string
	Port string
}

func GetDefaultLocalGrpcClient() GrpcClient {
	return GrpcClient{
		Cfg: GrpcClientConfig{
			Host: "127.0.0.1",
			Port: "8182",
		},
	}
}

func (c GrpcClient) GetConnection() (*grpc.ClientConn, error) {
	return grpc.Dial(
		fmt.Sprintf("%s:%s", c.Cfg.Host, c.Cfg.Port),
		grpc.WithTransportCredentials(insecure.NewCredentials()),
	)
}

func (c GrpcClient) SaveStages(ctx context.Context, uuid string, isFinal bool, stages []models.Stage) (err error) {
	conn, err := c.GetConnection()

	if err != nil {
		return
	}

	defer conn.Close()
	client := generated.NewStagesServiceClient(conn)

	record := &generated.SaveStagesRequest{
		ClientId: uuid,
		IsFinal:  isFinal,
		Stages:   []*generated.Stage{},
	}

	for _, s := range stages {
		stage := s.ToProto()
		record.Stages = append(record.Stages, stage)
	}

	_, err = client.SaveStages(ctx, record)
	return
}
