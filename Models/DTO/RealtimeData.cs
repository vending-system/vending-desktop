using System;
using System.Collections.Generic;

namespace VendingSystemClient.Models.DTO;

public class RealtimeData
{
    public RealtimeConnection? Connection { get; set; }
    public RealtimeLoad? Load { get; set; }
    public RealtimeCash? Cash { get; set; }
    public List<RealtimeStatus>? Statuses { get; set; }
    public List<RealtimeEvent>? Events { get; set; }
}

public class RealtimeConnection
{
    public string Status { get; set; } = "";      // "online" / "offline"
    public string? Provider { get; set; }          // "MegaFon", "T2" и т.д.
    public int Signal { get; set; }                // 0-100
    public DateTime? LastSeen { get; set; }
}

public class RealtimeLoad
{
    public int Coffee { get; set; }
    public int Sugar { get; set; }
    public int Milk { get; set; }
    public int Cups { get; set; }
    public int Overall { get; set; }               // общая загрузка %
}

public class RealtimeCash
{
    public decimal FullMoney { get; set; }
    public int Coins { get; set; }
    public int Bills { get; set; }
    public bool ChangeAvailable { get; set; }
}

public class RealtimeStatus
{
    public string Code { get; set; } = "";
    public string Message { get; set; } = "";
    public string Type { get; set; } = "";         // "info" / "warning" / "error"
}

public class RealtimeEvent
{
    public string Time { get; set; } = "";
    public string Type { get; set; } = "";         // "sale"
    public string? Product { get; set; }
    public decimal Amount { get; set; }
}