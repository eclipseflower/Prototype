// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "Kismet/GameplayStatics.h"
#include "Engine/StaticMeshActor.h"
#include "FloatingActor.generated.h"

struct Plane
{
	FVector center;
	FVector normal;
};

UCLASS()
class PROTOTYPE_API AFloatingActor : public AActor
{
	GENERATED_BODY()
	
public:	
	// Sets default values for this actor's properties
	AFloatingActor();

	void Initialize();
	void ConstructPlane();
	void GenTarget();

	UPROPERTY(VisibleAnywhere)
	UStaticMeshComponent* VisualMesh;

	UPROPERTY(VisibleAnywhere)
	UMaterialInstanceDynamic* DynamicMaterialInst;

	AStaticMeshActor* pSphereActor;

	Plane planes[6];
	FVector targetPos;
	FRotator targetRot;
	FVector targetScale;
	bool away;
	float moveSpeed;
	float rotSpeed;
	float scaleSpeed;

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

public:	
	// Called every frame
	virtual void Tick(float DeltaTime) override;

};
