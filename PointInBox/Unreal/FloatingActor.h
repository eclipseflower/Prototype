// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "FloatingActor.generated.h"

struct Plane
{
	FVector center;
	FVector normal;
};


UCLASS()
class PROTOTYPE_UNREAL_API AFloatingActor : public AActor
{
	GENERATED_BODY()
	
public:	
	// Sets default values for this actor's properties
	AFloatingActor();
	void ConstructPlane();

	UPROPERTY(VisibleAnywhere)
	UStaticMeshComponent* VisualMesh;

	Plane planes[6];

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

public:	
	// Called every frame
	virtual void Tick(float DeltaTime) override;

};

