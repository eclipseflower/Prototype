// Fill out your copyright notice in the Description page of Project Settings.


#include "FloatingActor.h"

// Sets default values
AFloatingActor::AFloatingActor()
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

	away = true;
	moveSpeed = 1.0f;
	rotSpeed = 1.0f;
	scaleSpeed = 1.0f;
}

void AFloatingActor::GenTarget()
{
	if (away)
	{
		targetPos = FVector(FMath::RandRange(0.0f, 1000.0f), FMath::RandRange(0.0f, 1000.0f), FMath::RandRange(0.0f, 1000.0f));
	}
	else
	{
		targetPos = FVector::ZeroVector;
	}
	targetRot = FRotator(FMath::RandRange(-180.0f, 180.0f), FMath::RandRange(-180.0f, 180.0f), FMath::RandRange(-180.0f, 180.0f));
	targetScale = FVector(FMath::RandRange(1.0f, 10.0f), FMath::RandRange(1.0f, 10.0f), FMath::RandRange(1.0f, 10.0f));
}

void AFloatingActor::Initialize()
{
	TArray<AActor*> actorsToFind;
	UGameplayStatics::GetAllActorsOfClassWithTag(GetWorld(), AStaticMeshActor::StaticClass(), FName("Sphere"), actorsToFind);

	pSphereActor = nullptr;
	for (AActor* actor: actorsToFind)
    {
        pSphereActor = Cast<AStaticMeshActor>(actor);

        if (pSphereActor)
        {
			break;
        }   
    }

	if (!pSphereActor)
	{
		UE_LOG(LogTemp, Log, TEXT("ACTOR NULLPTR!!!!"));
	}
}

void AFloatingActor::ConstructPlane()
{
	FVector pos = GetActorLocation();
	FRotator rot = GetActorRotation();
	FVector scale = GetActorScale();

	FMatrix posMat = FTranslationMatrix(pos);
	FMatrix rotMat = FRotationMatrix(rot);
	FMatrix mat = posMat * rotMat;

	auto x = scale[0] * 0.5 * 100;
	auto y = scale[1] * 0.5 * 100;
	auto z = scale[2] * 0.5 * 100;

	planes[0].center = FVector(-x, 0, 0);
	planes[1].center = FVector(+x, 0, 0);
	planes[2].center = FVector(0, -y, 0);
	planes[3].center = FVector(0, +y, 0);
	planes[4].center = FVector(0, 0, -z);
	planes[5].center = FVector(0, 0, +z);

	planes[0].normal = FVector(+1, 0, 0);
	planes[1].normal = FVector(-1, 0, 0);
	planes[2].normal = FVector(0, +1, 0);
	planes[3].normal = FVector(0, -1, 0);
	planes[4].normal = FVector(0, 0, +1);
	planes[5].normal = FVector(0, 0, -1);

	for(int i = 0; i < 6; i++)
	{
		planes[i].center = mat.TransformPosition(planes[i].center);
		planes[i].normal = (mat.TransformVector(planes[i].normal)).GetUnsafeNormal3();
	}
}

// Called when the game starts or when spawned
void AFloatingActor::BeginPlay()
{
	Super::BeginPlay();
	Initialize();
	ConstructPlane();
	GenTarget();
}

// Called every frame
void AFloatingActor::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

	if (!pSphereActor)
	{
		return;
	}

	FVector pos = pSphereActor->GetActorLocation();
	ConstructPlane();

	bool hit = true;
	for(int i = 0; i < 6; i++)
	{
		auto res = FVector::DotProduct(pos - planes[i].center, planes[i].normal);
		if(res <= 0)
		{
			hit = false;
			break;
		}
	}

	if (DynamicMaterialInst)
	{
		if (hit)
		{
			DynamicMaterialInst->SetVectorParameterValue(TEXT("Color"), FVector4(1, 0, 0, 0));
		}
		else
		{
			DynamicMaterialInst->SetVectorParameterValue(TEXT("Color"), FVector4(1, 1, 1, 0));
		}
	}

	FVector NewLocation = GetActorLocation();
	FRotator NewRotation = GetActorRotation();
	FVector NewScale = GetActorScale();

	NewLocation = FMath::Lerp(NewLocation, targetPos, moveSpeed * DeltaTime);
	NewRotation = FQuat::Slerp(NewRotation.Quaternion(), targetRot.Quaternion(), rotSpeed * DeltaTime).Rotator();
	NewScale = FMath::Lerp(NewScale, targetScale, scaleSpeed * DeltaTime);
	SetActorLocationAndRotation(NewLocation, NewRotation);
	SetActorRelativeScale3D(NewScale);

	FVector posDelta = NewLocation - targetPos;
	if (posDelta.SquaredLength() < 10.0f)
	{
		away = !away;
		GenTarget();
	}
}
