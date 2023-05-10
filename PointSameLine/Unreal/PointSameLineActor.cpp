// Fill out your copyright notice in the Description page of Project Settings.


#include "PointSameLineActor.h"

// Sets default values
APointSameLineActor::APointSameLineActor()
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

	VisualMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("Mesh"));
	VisualMesh->SetupAttachment(RootComponent);

	static ConstructorHelpers::FObjectFinder<UStaticMesh> CubeVisualAsset(TEXT("/Engine/BasicShapes/Cube"));

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

	pActor1 = nullptr;
	pActor2 = nullptr;
	speed1 = 1.0f;
	speed2 = 1.0f;
	axis = FVector::YAxisVector;
	hitTime = 0.0f;
}

void APointSameLineActor::Generate()
{
	FVector r = pActor1->GetActorLocation();

	speed1 = FMath::RandRange(10.0f, 100.0f);
	speed2 = FMath::RandRange(10.0f, 100.0f);

	auto x = FGenericPlatformMath::Abs(r.X);
	auto y = FGenericPlatformMath::Abs(r.Y);
	auto z = FGenericPlatformMath::Abs(r.Z);

	if(x > y)
	{
		if(y > z)
		{
			axis = FVector(-r.y, r.x, 0);
		}
		else
		{
			axis = FVector(-r.z, 0, r.x);
		}
	}
	else
	{
		if(x > z)
		{
			axis = FVector(r.y, -r.x, 0);
		}
		else
		{
			axis = FVector(0, -r.z, r.y);
		}
	}
	axis = axis.GetUnsafeNormal();
}

void APointSameLineActor::Rotate()
{
	FVector r1 = pActor1->GetActorLocation();
	FVector rv1 = r1.RotateAngleAxis(speed1 * DeltaTime, axis);
	r1.X += rv1.X;
	r1.Y += rv1.Y;
	r1.Z += rv1.Z;

	pActor1->SetActorLocation(r1);
}

// Called when the game starts or when spawned
void APointSameLineActor::BeginPlay()
{
	Super::BeginPlay();
	Generate();
}

// Called every frame
void APointSameLineActor::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
	Rotate();
}

