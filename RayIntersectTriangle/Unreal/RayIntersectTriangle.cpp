// Fill out your copyright notice in the Description page of Project Settings.


#include "RayIntersectTriangle.h"

// Sets default values
ARayIntersectTriangle::ARayIntersectTriangle()
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;
	CreateMesh();
}

// Called when the game starts or when spawned
void ARayIntersectTriangle::BeginPlay()
{
	Super::BeginPlay();
	p1 = pActor1->GetActorLocation();
	p2 = pActor2->GetActorLocation();
	p3 = pActor3->GetActorLocation();
	normal = FVector::CrossProduct(p2 - p1, p3 - p1);
}

void ARayIntersectTriangle::CreateMesh()
{
	pVisualMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("Mesh"));
	pVisualMesh->SetupAttachment(RootComponent);

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

	UProceduralMeshComponent* pProceduralMesh = nullptr;
	pmc = CreateDefaultSubobject<UProceduralMeshComponent>("pmc");
	pmc->SetupAttachment(RootComponent);
	Verties.Add(FVector(0, 0, 0));
	Verties.Add(FVector(200, 0, 0));
	Verties.Add(FVector(0, 0, 100));
	Triangles.Add(2);
	Triangles.Add(1);
	Triangles.Add(0);

	pmc->CreateMeshSection_LinearColor(0, Verties, Triangles, TArray<FVector>(),
			TArray<FVector2D>(), TArray<FVector2D>(), TArray<FVector2D>(), TArray<FVector2D>(),
			TArray<FLinearColor>(), TArray<FProcMeshTangent>(), true);
}

// Called every frame
void ARayIntersectTriangle::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

}

