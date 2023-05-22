// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "Engine/StaticMeshActor.h"
#include "ProceduralMeshComponent.h"
#include "RayIntersectTriangle.generated.h"

UCLASS()
class PROTOTYPE_API ARayIntersectTriangle : public AActor
{
	GENERATED_BODY()
	
public:	
	// Sets default values for this actor's properties
	ARayIntersectTriangle();

	UPROPERTY(EditAnywhere)
	AStaticMeshActor* pActor1;

	UPROPERTY(EditAnywhere)
	AStaticMeshActor* pActor2;

	UPROPERTY(EditAnywhere)
	AStaticMeshActor* pActor3;

	UPROPERTY(VisibleAnywhere)
	UStaticMeshComponent* pVisualMesh;

	UPROPERTY(VisibleAnywhere)
	UMaterialInstanceDynamic* pDynamicMaterialInst;

private:
	FVector p1;
	FVector p2;
	FVector p3;
	FVector normal;


protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;
	void CreateMesh();
	void CreateTriangle();

public:	
	// Called every frame
	virtual void Tick(float DeltaTime) override;

};
