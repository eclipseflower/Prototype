// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "PointSameLineActor.generated.h"

UCLASS()
class PROTOTYPE_API APointSameLineActor : public AActor
{
	GENERATED_BODY()

public:
	// Sets default values for this actor's properties
	APointSameLineActor();

	UPROPERTIES(EditAnywhere)
	APointSameLineActor* pActor1;

	UPROPERTIES(EditAnywhere)
	APointSameLineActor* pActor2;

	float speed1;
	float speed2;
	FVector axis;
	float hitTime;

	UPROPERTY(VisibleAnywhere)
	UStaticMeshComponent* VisualMesh;

	UPROPERTY(VisibleAnywhere)
	UMaterialInstanceDynamic* DynamicMaterialInst;

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

	void Generate();
	void Rotate();

public:
	// Called every frame
	virtual void Tick(float DeltaTime) override;

};
