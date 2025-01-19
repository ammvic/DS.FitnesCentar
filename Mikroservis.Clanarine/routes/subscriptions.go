package routes

import (
	"github.com/gin-gonic/gin"
	"mikroservis.clanarine/controllers" //  paket kontrolera
)

func SubscriptionRoutes(router *gin.Engine, controller *controllers.SubscriptionsController) {
	subscriptionRoutes := router.Group("/api/subscriptions")
	{
		subscriptionRoutes.GET("", controller.GetAll)
		subscriptionRoutes.GET("/:id", controller.GetById)
		subscriptionRoutes.POST("", controller.Create)
		subscriptionRoutes.PUT("/:id", controller.Update)
		subscriptionRoutes.DELETE("/:id", controller.Delete)
	}
}
