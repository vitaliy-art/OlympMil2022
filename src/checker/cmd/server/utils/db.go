package utils

import (
	"checker/src/checker/models"

	"github.com/glebarez/sqlite"
	"gorm.io/gorm"
)

var db *gorm.DB

func init() {
	var err error

	if db, err = gorm.Open(sqlite.Open("checker_server_bd.db"), &gorm.Config{}); err != nil {
		panic(err)
	}

	db.AutoMigrate(
		&models.Record{},
	)
}

func GetDB() *gorm.DB {
	return db
}
