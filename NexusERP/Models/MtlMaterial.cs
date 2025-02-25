using System;
using System.Collections.Generic;

namespace NexusERP.Models;

public partial class MtlMaterial
{
    public int Id { get; set; }

    public string MaterialId { get; set; } = null!;

    public string? CsbMaterialId { get; set; }

    public string? MaterialDesc { get; set; }

    public string? InventoryUom { get; set; }

    public string? PriceUomZ { get; set; }

    public string? OrderUomZ { get; set; }

    public string? PriceUomS { get; set; }

    public string? OrderUomS { get; set; }

    public string? ReceiptClass { get; set; }

    public string? MaterialType { get; set; }

    public string? ProductType { get; set; }

    public string? ProdQcStatus { get; set; }

    public string? ProdQcAttribute { get; set; }

    public string? RegCertNo1 { get; set; }

    public string? RegCertNo2 { get; set; }

    public string? StorageCond { get; set; }

    public string? TradeItemIdNo { get; set; }

    public string? ContainerTrack { get; set; }

    public string? ExtLotFlag { get; set; }

    public string? Api { get; set; }

    public double? ExpirationRule { get; set; }

    public string? DefTransfer { get; set; }

    public double? DefTransferCoef { get; set; }

    public int? ActiveFlag { get; set; }

    public string? MaterialDescShort { get; set; }

    public string? Ean { get; set; }

    public int? MtlGroup { get; set; }

    public string? MsdsName { get; set; }

    public string? MsdsVer { get; set; }

    public string? SopName { get; set; }

    public string? SopVer { get; set; }

    public string? SpecName { get; set; }

    public string? SpecVer { get; set; }

    public string? TMaterialId { get; set; }

    public int? MtlSubgroup { get; set; }

    public int? WhseGeoloc { get; set; }

    public string? MaterialDescDeEng { get; set; }

    public string? MaterialDescExtra { get; set; }

    public int? OrgnCode { get; set; }

    public double? ExpirationRuleMin { get; set; }

    public double? ExpirationRuleMax { get; set; }

    public string? BrandProduct { get; set; }

    public string? CommentClient { get; set; }

    public string? Comment { get; set; }

    public string? CommentProduction { get; set; }

    public string? Flags { get; set; }

    public int? AreaId { get; set; }

    public int? PckgGroup { get; set; }

    public double? AlarmQtyMin { get; set; }

    public double? ExpirationRuleOld { get; set; }
}
