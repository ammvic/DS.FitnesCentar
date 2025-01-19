package repository

import (
	"log"
	"context"
	"errors"

	"mikroservis.clanarine/models"

	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"
)

type SubscriptionRepository struct {
	collection *mongo.Collection
}

func NewSubscriptionRepository(db *mongo.Database) *SubscriptionRepository {
	return &SubscriptionRepository{
		collection: db.Collection("subscriptions"),
	}
}

func (r *SubscriptionRepository) GetAll(ctx context.Context) ([]models.MemberSubscription, error) {
	var subscriptions []models.MemberSubscription
	cursor, err := r.collection.Find(ctx, bson.M{})
	if err != nil {
		return nil, err
	}
	defer cursor.Close(ctx)

	if err = cursor.All(ctx, &subscriptions); err != nil {
		return nil, err
	}
	return subscriptions, nil
}

func (r *SubscriptionRepository) GetById(ctx context.Context, id string) (*models.MemberSubscription, error) {
	var subscription models.MemberSubscription
	err := r.collection.FindOne(ctx, bson.M{"_id": id}).Decode(&subscription)
	if err != nil {
		if errors.Is(err, mongo.ErrNoDocuments) {
			return nil, nil
		}
		return nil, err
	}
	return &subscription, nil
}

func (r *SubscriptionRepository) Create(ctx context.Context, subscription *models.MemberSubscription) error {
	 log.Println("Subscription data:", subscription)
	_, err := r.collection.InsertOne(ctx, subscription)
	return err
}

func (r *SubscriptionRepository) Update(ctx context.Context, id string, subscription *models.MemberSubscription) error {
	update := bson.M{
		"$set": subscription,
	}
	result, err := r.collection.UpdateOne(ctx, bson.M{"_id": id}, update)
	if err != nil {
		return err
	}
	if result.MatchedCount == 0 {
		return errors.New("no document found to update")
	}
	return nil
}

func (r *SubscriptionRepository) Delete(ctx context.Context, id string) error {
	result, err := r.collection.DeleteOne(ctx, bson.M{"_id": id})
	if err != nil {
		return err
	}
	if result.DeletedCount == 0 {
		return errors.New("no document found to delete")
	}
	return nil
}
