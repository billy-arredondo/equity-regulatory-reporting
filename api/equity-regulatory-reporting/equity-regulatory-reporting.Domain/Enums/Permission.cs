namespace equity_regulatory_reporting.Domain.Enums;

[Flags]
public enum Permission
{
    None = 0,

    // Persons
    PersonRead   = 1 << 0,  // 1
    PersonWrite  = 1 << 1,  // 2
    PersonDelete = 1 << 2,  // 4

    // Participations
    ParticipationRead   = 1 << 3,  // 8
    ParticipationWrite  = 1 << 4,  // 16
    ParticipationDelete = 1 << 5,  // 32

    // Board of directors
    BoardRead   = 1 << 6,  // 64
    BoardWrite  = 1 << 7,  // 128
    BoardDelete = 1 << 8,  // 256

    // Reports
    ReportRead     = 1 << 9,   // 512
    ReportGenerate = 1 << 10,  // 1024

    // Users
    UserRead   = 1 << 11,  // 2048
    UserWrite  = 1 << 12,  // 4096
    UserDelete = 1 << 13,  // 8192

    // Countries
    CountryRead   = 1 << 14,  // 16384
    CountryWrite  = 1 << 15,  // 32768
    CountryDelete = 1 << 16,  // 65536

    // Document types
    DocumentTypeRead   = 1 << 17,  // 131072
    DocumentTypeWrite  = 1 << 18,  // 262144
    DocumentTypeDelete = 1 << 19,  // 524288

    // Positions
    PositionRead   = 1 << 20,  // 1048576
    PositionWrite  = 1 << 21,  // 2097152
    PositionDelete = 1 << 22,  // 4194304

    Admin = ~0
}
