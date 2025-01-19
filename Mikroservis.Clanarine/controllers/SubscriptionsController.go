package controllers

import (
	"context"
	"net/http"
	"time"

	"github.com/gin-gonic/gin"
	"mikroservis.clanarine/models"
	subscriptionRepo "mikroservis.clanarine/repository"
)

type SubscriptionsController struct {
	repository *subscriptionRepo.SubscriptionRepository
}

func NewSubscriptionsController(repo *subscriptionRepo.SubscriptionRepository) *SubscriptionsController {
	return &SubscriptionsController{
		repository: repo,
	}
}

// GetAll retrieves all subscriptions
func (sc *SubscriptionsController) GetAll(c *gin.Context) {
	subscriptions, err := sc.repository.GetAll(context.Background())
	if err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
		return
	}
	c.JSON(http.StatusOK, subscriptions)
}

// GetById retrieves a subscription by ID
func (sc *SubscriptionsController) GetById(c *gin.Context) {
	id := c.Param("id")
	subscription, err := sc.repository.GetById(context.Background(), id)
	if err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
		return
	}
	if subscription == nil {
		c.JSON(http.StatusNotFound, gin.H{"message": "Subscription not found"})
		return
	}
	c.JSON(http.StatusOK, subscription)
}

// Create creates a new subscription
func (sc *SubscriptionsController) Create(c *gin.Context) {
	var subscription models.MemberSubscription
	if err := c.ShouldBindJSON(&subscription); err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
		return
	}

	subscription.Id = ""
	subscription.StartDate = time.Now()
	if err := sc.repository.Create(context.Background(), &subscription); err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
		return
	}
	c.JSON(http.StatusCreated, subscription)
}

// Update updates an existing subscription
func (sc *SubscriptionsController) Update(c *gin.Context) {
	id := c.Param("id")
	var subscription models.MemberSubscription
	if err := c.ShouldBindJSON(&subscription); err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
		return
	}

	if err := sc.repository.Update(context.Background(), id, &subscription); err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
		return
	}
	c.JSON(http.StatusNoContent, nil)
}

// Delete deletes a subscription by ID
func (sc *SubscriptionsController) Delete(c *gin.Context) {
	id := c.Param("id")
	if err := sc.repository.Delete(context.Background(), id); err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
		return
	}
	c.JSON(http.StatusNoContent, nil)
}
