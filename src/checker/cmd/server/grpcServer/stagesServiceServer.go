package grpcServer

import (
	"checker/src/checker/cmd/server/grpc"
	"checker/src/checker/cmd/server/utils"
	"checker/src/checker/models"
	"context"
)

type StagesServiceServerImpl struct {
	*grpc.UnimplementedStagesServiceServer
}

func (StagesServiceServerImpl) SaveStages(ctx context.Context, req *grpc.SaveStagesRequest) (*grpc.Empty, error) {
	db := utils.GetDB()
	stages := []models.Stage{}

	for _, s := range req.Stages {
		stage := &models.Stage{}
		stage.FromProto(s)
		stages = append(stages, *stage)
	}

	record := &models.Record{}
	db.Find(record, &models.Record{Uuid: req.ClientId})

	if err := record.SetStages(stages); err != nil {
		return nil, err
	}

	if record.Uuid == "" {
		record.Uuid = req.ClientId
		return &grpc.Empty{}, db.Create(record).Error
	} else {
		return &grpc.Empty{}, db.Save(record).Error
	}
}
