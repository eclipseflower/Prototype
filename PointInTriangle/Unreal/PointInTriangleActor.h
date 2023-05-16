// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "Engine/StaticMeshActor.h"
#include "PointInTriangleActor.generated.h"

UCLASS()
class PROTOTYPE_API APointInTriangleActor : public AActor
{
	GENERATED_BODY()
	
public:	
	// Sets default values for this actor's properties
	APointInTriangleActor();

	UPROPERTY(EditAnywhere)
	AStaticMeshActor* pActor1;

	UPROPERTY(EditAnywhere)
	AStaticMeshActor* pActor2;

	UPROPERTY(EditAnywhere)
	AStaticMeshActor* pActor3;

	UPROPERTY(EditAnywhere)
	float moveSpeed;

	FVector normal;
	FVector p1;
	FVector p2;
	FVector p3;
	FVector targetPos;
	bool away;

	UPROPERTY(VisibleAnywhere)
	UStaticMeshComponent* VisualMesh;

	UPROPERTY(VisibleAnywhere)
	UMaterialInstanceDynamic* DynamicMaterialInst;

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;
	void GenTarget();

public:	
	// Called every frame
	virtual void Tick(float DeltaTime) override;

};
