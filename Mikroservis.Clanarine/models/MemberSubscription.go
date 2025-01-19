package models

import (
	"time"
)

type MemberSubscription struct {
	Id        string    `bson:"_id,omitempty"` // MongoDB koristi string kao ObjectId
	MemberId  string    `bson:"member_id"`
	Type      string    `bson:"type"` // Npr. "Monthly", "Yearly"
	Price     float64   `bson:"price"`
	StartDate time.Time `bson:"start_date"`
	EndDate   time.Time `bson:"end_date"`
	IsActive  bool      `bson:"is_active"`
}
