package main

import (
	"log"
	"mikroservis.clanarine/configs"
	"mikroservis.clanarine/controllers"
	"mikroservis.clanarine/repository" 
	"mikroservis.clanarine/routes"

	"github.com/gin-gonic/gin"
	
)

func main() {
	// Povezivanje s bazom
	client, err := configs.ConnectDB()
	if err != nil {
		log.Fatalf("Failed to connect to the database: %v", err)
	}
	defer client.Disconnect(nil)

	// Dobijanje baze podataka iz MongoDB klijenta
	db := client.Database("Clanovi")  

	// Inicijalizacija repozitorijuma i kontrolera
	subscriptionRepo := repository.NewSubscriptionRepository(db)  // Prosleđivanje baze podataka
	subscriptionController := controllers.NewSubscriptionsController(subscriptionRepo)

	// Postavljanje ruta
	router := gin.Default()
	routes.SubscriptionRoutes(router, subscriptionController)

	// Pokretanje servera
	log.Println("Server running on port 8080")
	if err := router.Run(":8080"); err != nil {
		log.Fatalf("Failed to run server: %v", err)
	}
}
