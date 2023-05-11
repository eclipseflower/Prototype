// Fill out your copyright notice in the Description page of Project Settings.


#include "PointSameLineActor.h"

// Sets default values
APointSameLineActor::APointSameLineActor()
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

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

	isMain = false;
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
			axis = FVector(-r.Y, r.X, 0);
		}
		else
		{
			axis = FVector(-r.Z, 0, r.X);
		}
	}
	else
	{
		if(x > z)
		{
			axis = FVector(r.Y, -r.X, 0);
		}
		else
		{
			axis = FVector(0, -r.Z, r.Y);
		}
	}
	axis = axis.GetUnsafeNormal();
}

void APointSameLineActor::Rotate(float DeltaTime)
{
	FVector r1 = pActor1->GetActorLocation();
	r1 = r1.RotateAngleAxis(speed1 * DeltaTime, axis);

	pActor1->SetActorLocation(r1);

	FVector r2 = pActor2->GetActorLocation();
	r2 = r2.RotateAngleAxis(speed2 * DeltaTime, axis);

	pActor2->SetActorLocation(r2);

}

// Called when the game starts or when spawned
void APointSameLineActor::BeginPlay()
{
	Super::BeginPlay();
	if (!isMain)
	{
		return;
	}
	Generate();
}

// Called every frame
void APointSameLineActor::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
	if (!isMain)
	{
		return;
	}

	Rotate(DeltaTime);

	FVector p1 = GetActorLocation();
	FVector p2 = pActor1->GetActorLocation();
	FVector p3 = pActor2->GetActorLocation();

	FVector l1 = p2 - p1;
	FVector l2 = p3 - p1;

	FVector res = l1.GetUnsafeNormal() ^ l2.GetUnsafeNormal();
	UE_LOG(LogTemp, Warning, TEXT("cross product:%f"), res.SquaredLength());
	if (res.SquaredLength() < 0.0001f)
	{
		DynamicMaterialInst->SetVectorParameterValue(TEXT("Color"), FVector4(1, 0, 0, 0));
		pActor1->DynamicMaterialInst->SetVectorParameterValue(TEXT("Color"), FVector4(1, 0, 0, 0));
		pActor2->DynamicMaterialInst->SetVectorParameterValue(TEXT("Color"), FVector4(1, 0, 0, 0));
		hitTime = GetGameTimeSinceCreation();
		Generate();
	}
	else
	{
		if (FGenericPlatformMath::Abs(GetGameTimeSinceCreation() - hitTime) < 0.5f)
		{
			DynamicMaterialInst->SetVectorParameterValue(TEXT("Color"), FVector4(1, 0, 0, 0));
			pActor1->DynamicMaterialInst->SetVectorParameterValue(TEXT("Color"), FVector4(1, 0, 0, 0));
			pActor2->DynamicMaterialInst->SetVectorParameterValue(TEXT("Color"), FVector4(1, 0, 0, 0));
		}
		else
		{
			DynamicMaterialInst->SetVectorParameterValue(TEXT("Color"), FVector4(1, 1, 1, 0));
			pActor1->DynamicMaterialInst->SetVectorParameterValue(TEXT("Color"), FVector4(1, 1, 1, 0));
			pActor2->DynamicMaterialInst->SetVectorParameterValue(TEXT("Color"), FVector4(1, 1, 1, 0));
		}
	}
}

