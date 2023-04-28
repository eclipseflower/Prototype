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

	away = true;
	moveSpeed = 1.0f;
	rotSpeed = 1.0f;
	scaleSpeed = 1.0f;
}

void AFloatingActor::GenTarget()
{
	if (away)
	{
		targetPos = FVector(FMath::RandRange(0.0f, 10.0f), FMath::RandRange(0.0f, 10.0f), FMath::RandRange(0.0f, 10.0f));
	}
	else
	{
		targetPos = FVector::ZeroVector;
	}
	targetRot = FRotator(FMath::RandRange(-1.0f, 1.0f), FMath::RandRange(-1.0f, 1.0f), FMath::RandRange(-1.0f, 1.0f));
	targetScale = FVector(FMath::RandRange(0.0f, 10.0f), FMath::RandRange(0.0f, 10.0f), FMath::RandRange(0.0f, 10.0f));
}

void AFloatingActor::Initialize()
{
	TArray<AActor*> actorsToFind;
	UGameplayStatics::GetAllActorsOfClassWithTag(GetWorld(), AStaticMeshActor::StaticClass(), FName("Sphere"), actorsToFind);

	for (AActor* actor: actorsToFind)
    {
        pSphereActor = Cast<AStaticMeshActor>(actor);

        if (pSphereActor)
        {
			break;
        }   
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

	auto x = scale[0] * 0.5;
	auto y = scale[1] * 0.5;
	auto z = scale[2] * 0.5;

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

	FVector NewLocation = GetActorLocation();
	FRotator NewRotation = GetActorRotation();
	float RunningTime = GetGameTimeSinceCreation();
	float DeltaHeight = (FMath::Sin(RunningTime + DeltaTime) - FMath::Sin(RunningTime));
	NewLocation.Z += DeltaHeight * 20.0f;       //Scale our height by a factor of 20
	float DeltaRotation = DeltaTime * 20.0f;    //Rotate by 20 degrees per second
	NewRotation.Yaw += DeltaRotation;
	SetActorLocationAndRotation(NewLocation, NewRotation);

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
}

