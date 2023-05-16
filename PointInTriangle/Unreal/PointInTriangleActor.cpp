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

	VisualMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("Mesh"));
	VisualMesh->SetupAttachment(RootComponent);

	static ConstructorHelpers::FObjectFinder<UStaticMesh> CubeVisualAsset(TEXT("/Engine/BasicShapes/Sphere"));

	if (CubeVisualAsset.Succeeded())
	{
		VisualMesh->SetStaticMesh(CubeVisualAsset.Object);
		VisualMesh->SetRelativeLocation(FVector(0.0f, 0.0f, 0.0f));
	}

	static ConstructorHelpers::FObjectFinder<UMaterial> CubeMaterialAsset(TEXT("/Game/StarterContent/Materials/M_Basic_Floor"));

	if (CubeMaterialAsset.Succeeded())
	{
		DynamicMaterialInst = UMaterialInstanceDynamic::Create(CubeMaterialAsset.Object, VisualMesh);
		VisualMesh->SetMaterial(0, DynamicMaterialInst);
	}
}

// Called when the game starts or when spawned
void APointInTriangleActor::BeginPlay()
{
	Super::BeginPlay();
	p1 = pActor1->GetActorLocation();
	p2 = pActor2->GetActorLocation();
	p3 = pActor3->GetActorLocation();

	GenTarget();
}

void APointInTriangleActor::GenTarget()
{
	if (away)
	{
		targetPos = FVector(FMath::RandRange(-5000.0f, 5000.0f), FMath::RandRange(-4000.0f, 5000.0f), 0);
	}
	else
	{
		targetPos = FVector::ZeroVector;
	}
}

// Called every frame
void APointInTriangleActor::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

	FVector p = GetActorLocation();
	float r1 = ((p2 - p) ^ (p1 - p)) | normal;
	float r2 = ((p3 - p) ^ (p2 - p)) | normal;
	float r3 = ((p1 - p) ^ (p3 - p)) | normal;

	if (r1 > 0 && r2 > 0 && r3 > 0)
	{
		DynamicMaterialInst->SetVectorParameterValue(TEXT("Color"), FVector4(1, 0, 0, 0));
	}
	else
	{
		DynamicMaterialInst->SetVectorParameterValue(TEXT("Color"), FVector4(1, 1, 1, 0));
	}

	FVector NewLocation = GetActorLocation();

	NewLocation = FMath::Lerp(NewLocation, targetPos, moveSpeed * DeltaTime);
	SetActorLocation(NewLocation);
	FVector posDelta = NewLocation - targetPos;
	if (posDelta.SquaredLength() < 10.0f)
	{
		away = !away;
		GenTarget();
	}
}

