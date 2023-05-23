// Fill out your copyright notice in the Description page of Project Settings.


#include "RayIntersectTriangle.h"

// Sets default values
ARayIntersectTriangle::ARayIntersectTriangle()
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

	pVisualMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("Mesh"));
	pVisualMesh->SetupAttachment(RootComponent);

	pProceduralMesh = CreateDefaultSubobject<UProceduralMeshComponent>("ProcMesh");
	pProceduralMesh->SetupAttachment(RootComponent);
}

// Called when the game starts or when spawned
void ARayIntersectTriangle::BeginPlay()
{
	Super::BeginPlay();
	p1 = pActor1->GetActorLocation();
	p2 = pActor2->GetActorLocation();
	p3 = pActor3->GetActorLocation();
	normal = FVector::CrossProduct(p2 - p1, p3 - p1);

	CreateMesh();
	CreateTriangle();
}

void ARayIntersectTriangle::CreateMesh()
{

	static ConstructorHelpers::FObjectFinder<UStaticMesh> cubeVisualAsset(TEXT("/Engine/BasicShapes/Sphere"));

	if (cubeVisualAsset.Succeeded())
	{
		pVisualMesh->SetStaticMesh(cubeVisualAsset.Object);
		pVisualMesh->SetRelativeLocation(FVector(0.0f, 0.0f, 0.0f));
	}

	static ConstructorHelpers::FObjectFinder<UMaterial> cubeMaterialAsset(TEXT("/Game/StarterContent/Materials/M_Basic_Floor"));

	if (cubeMaterialAsset.Succeeded())
	{
		pDynamicMaterialInst = UMaterialInstanceDynamic::Create(cubeMaterialAsset.Object, pVisualMesh);
		pVisualMesh->SetMaterial(0, pDynamicMaterialInst);
	}
}

void ARayIntersectTriangle::CreateTriangle()
{
	TArray<FVector> verties;
	TArray<int> triangles;

	verties.Add(p1);
	verties.Add(p2);
	verties.Add(p3);
	triangles.Add(0);
	triangles.Add(1);
	triangles.Add(2);

	pProceduralMesh->CreateMeshSection_LinearColor(0, verties, triangles, TArray<FVector>(),
			TArray<FVector2D>(), TArray<FVector2D>(), TArray<FVector2D>(), TArray<FVector2D>(),
			TArray<FLinearColor>(), TArray<FProcMeshTangent>(), true);
	pProceduralMesh->SetMaterial(0, pDynamicMaterialInst);
}

// Called every frame
void ARayIntersectTriangle::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

	FVector lineStart = GetActorLocation();
	FVector lineEnd = GetActorLocation() + GetActorForwardVector() * 1000.0f;
	DrawDebugLine(GetWorld(), lineStart, lineEnd, FColor::Blue, false, 0.0f, 0.0f, 10.0f);


}

