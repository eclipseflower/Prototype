// Fill out your copyright notice in the Description page of Project Settings.


#include "PointInTriangleActor.h"

// Sets default values
APointInTriangleActor::APointInTriangleActor()
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

	normal = FVector::ZAxisVector;
	away = true;
	moveSpeed = 1.0f;
}

// Called when the game starts or when spawned
void APointInTriangleActor::BeginPlay()
{
	Super::BeginPlay();
	
}

void APointInTriangleActor::GenTarget()
{
}

// Called every frame
void APointInTriangleActor::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

}

