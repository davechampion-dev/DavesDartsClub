﻿namespace DavesDartsClub.Domain;

public class Player
{
    public const int MemberNameMaxLength = 50;

    public Guid PlayerId { get; init; }

    public string PlayerName { get; init; }
}
