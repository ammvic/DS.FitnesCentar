package configs

import (
	"context"
	"fmt"
	"log"
	"time"

	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"
)

func ConnectDB() (*mongo.Client, error) {
	ctx, cancel := context.WithTimeout(context.Background(), 10*time.Second)
	defer cancel()

	client, err := mongo.Connect(ctx, options.Client().ApplyURI(EnvMongoURI()))
	if err != nil {
		return nil, err
	}

	// Ping the database
	err = client.Ping(ctx, nil)
	if err != nil {
		return nil, err
	}

	fmt.Println("Connected to MongoDB")
	return client, nil
}

// Getting database collections
func GetCollection(client *mongo.Client, collectionName string) *mongo.Collection {
	return client.Database("FitnessCentar").Collection(collectionName)
}

var DB *mongo.Client
var err error

func init() {
	DB, err = ConnectDB()
	if err != nil {
		log.Fatalf("Error connecting to the database: %v", err)
	}

	// Get a handle to the subscriptions collection
	subscriptionCollection := GetCollection(DB, "subscriptions")

	// Create unique index on subscription_id
	indexModel := mongo.IndexModel{
		Keys: bson.M{
			"member_id": 1, // index in ascending order
		},
		Options: options.Index().SetUnique(true),
	}

	_, err = subscriptionCollection.Indexes().CreateOne(context.Background(), indexModel)
	if err != nil {
		log.Fatal(err)
	}
}
