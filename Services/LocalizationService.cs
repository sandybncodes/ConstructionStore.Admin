using System.Globalization;

namespace ConstructionStore.Admin.Services;

public sealed class LocalizationService
{
    private static readonly TimeZoneInfo ChisinauTimeZone = ResolveChisinauTimeZone();

    private static readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Translations =
        new Dictionary<string, IReadOnlyDictionary<string, string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["ro"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["nav.main"] = "Principal",
                ["nav.catalogue"] = "Catalog",
                ["nav.sales"] = "Vanzari",
                ["nav.dashboard"] = "Panou principal",
                ["nav.products"] = "Produse",
                ["nav.categories"] = "Categorii",
                ["nav.orders"] = "Comenzi",
                ["dashboard.title"] = "Panou principal",
                ["dashboard.welcome"] = "Bine ai revenit",
                ["dashboard.products.description"] = "Adauga, editeaza si administreaza catalogul de produse",
                ["dashboard.categories.description"] = "Organizeaza produsele in categorii",
                ["dashboard.orders.description"] = "Vezi si actualizeaza statusurile comenzilor clientilor",
                ["common.loading"] = "Se incarca...",
                ["common.back"] = "Inapoi",
                ["common.cancel"] = "Anuleaza",
                ["common.save"] = "Salveaza",
                ["common.okClose"] = "Ok, inchide",
                ["common.all"] = "Toate",
                ["common.edit"] = "Editeaza",
                ["common.delete"] = "Sterge",
                ["common.clear"] = "Sterge selectia",
                ["common.none"] = "niciuna",
                ["common.noDescription"] = "Fara descriere",
                ["common.total"] = "Total",
                ["common.id"] = "ID",
                ["common.name"] = "Nume",
                ["common.description"] = "Descriere",
                ["common.category"] = "Categorie",
                ["common.images"] = "Imagini",
                ["common.price"] = "Pret",
                ["common.discount"] = "Reducere",
                ["common.stockQuantity"] = "Cantitate in stoc",
                ["common.active"] = "Activ",
                ["common.email"] = "Email",
                ["common.phone"] = "Telefon",
                ["common.notes"] = "Note",
                ["common.customer"] = "Client",
                ["common.date"] = "Data",
                ["common.items"] = "Articole",
                ["common.status"] = "Status",
                ["common.quantityAbbr"] = "Cant.",
                ["common.unitPrice"] = "Pret unitar",
                ["common.subtotal"] = "Subtotal",
                ["common.signOut"] = "Deconectare",
                ["common.currency"] = "MDL",
                ["products.title"] = "Produse",
                ["products.count"] = "produse",
                ["products.loading"] = "Se incarca produsele...",
                ["products.empty"] = "Nu au fost gasite produse.",
                ["products.search.placeholder"] = "Cauta dupa nume produs sau ID",
                ["products.stock"] = "Stoc",
                ["products.details"] = "Detalii produs",
                ["products.notFound"] = "Produsul nu a fost gasit.",
                ["products.addedOn"] = "Adaugat la",
                ["products.activeVisible"] = "Produsul este activ si vizibil clientilor",
                ["products.inactiveHidden"] = "Produsul este inactiv si ascuns",
                ["products.saved"] = "Produsul a fost salvat cu succes.",
                ["products.saveFailed"] = "Salvarea produsului a esuat.",
                ["products.addNew"] = "Adauga produs nou",
                ["products.addTitle"] = "Adauga produs",
                ["products.created"] = "Produsul a fost adaugat cu succes.",
                ["products.createFailed"] = "Adaugarea produsului a esuat.",
                ["products.imagesHelp"] = "Selecteaza una sau mai multe imagini. Prima imagine incarcata va deveni imaginea principala.",
                ["products.imagesSelected"] = "imagini selectate",
                ["products.imagesGallery"] = "Galerie imagini",
                ["products.imagesEmpty"] = "Produsul nu are imagini incarcate.",
                ["products.imagesManagerHelp"] = "Deschide o imagine din lista sau administreaza imaginile in modul de editare.",
                ["products.addImages"] = "Adauga imagini noi",
                ["products.chooseImages"] = "Alege imagini",
                ["products.uploadImages"] = "Incarca imagini",
                ["products.imagesUploaded"] = "Imaginile au fost incarcate cu succes.",
                ["products.imagesUploadFailed"] = "Incarcarea imaginilor a esuat.",
                ["products.imagesValidation"] = "Sunt acceptate doar imagini JPG, PNG, WEBP sau GIF.",
                ["products.imageDeleted"] = "Imaginea a fost stearsa.",
                ["products.imageDeleteFailed"] = "Stergerea imaginii a esuat.",
                ["products.deleteImageTitle"] = "Stergi aceasta imagine?",
                ["products.deleteImageMessage"] = "Imaginea selectata va fi eliminata din produs si stearsa din stocare. Aceasta actiune nu se poate anula.",
                ["products.deleteImageConfirm"] = "Sterge imaginea",
                ["products.mainImage"] = "Imagine principala",
                ["products.currentImage"] = "Imagine afisata",
                ["products.previousImage"] = "Imaginea anterioara",
                ["products.nextImage"] = "Imaginea urmatoare",
                ["products.imageNumber"] = "Imaginea {0}",
                ["products.viewImageNumber"] = "Vezi imaginea {0}",
                ["products.variants"] = "Variante produs",
                ["products.addVariant"] = "Adauga varianta",
                ["products.removeVariant"] = "Elimina varianta",
                ["products.variantNo"] = "Varianta {0}",
                ["products.variantPrice"] = "Pret varianta",
                ["products.variantStock"] = "Stoc varianta",
                ["products.variantSku"] = "SKU (optional)",
                ["products.variantActive"] = "Varianta activa",
                ["products.variantAttributes"] = "Atribute",
                ["products.variantAttributeValue"] = "Valoare",
                ["products.variantAttributeUnit"] = "Unitate",
                ["products.unitRequired"] = "Unitatea este obligatorie",
                ["products.variantsHint"] = "Adauga variante pentru dimensiuni, grosimi sau alte caracteristici diferite ale aceluiasi produs.",
                ["products.attributesLoadFailed"] = "Nu s-au putut incarca atributele. Variantele nu sunt disponibile.",
                ["products.variantSaved"] = "Varianta a fost salvata cu succes.",
                ["products.variantSaveFailed"] = "Salvarea variantei a esuat.",
                ["products.variantDeleted"] = "Varianta a fost stearsa.",
                ["products.variantDeleteFailed"] = "Stergerea variantei a esuat.",
                ["products.deleteVariantTitle"] = "Stergi aceasta varianta?",
                ["products.deleteVariantMessage"] = "Varianta selectata va fi eliminata definitiv din produs.",
                ["products.deleteVariantConfirm"] = "Sterge varianta",
                ["products.saveVariant"] = "Salveaza varianta",
                ["products.variantAdded"] = "Varianta a fost adaugata cu succes.",
                ["products.variantAddFailed"] = "Adaugarea variantei a esuat.",
                ["categories.title"] = "Categorii",
                ["categories.count"] = "categorii",
                ["categories.loading"] = "Se incarca categoriile...",
                ["categories.empty"] = "Nu au fost gasite categorii.",
                ["categories.search.placeholder"] = "Cauta dupa nume categorie sau ID",
                ["categories.details"] = "Detalii categorie",
                ["categories.notFound"] = "Categoria nu a fost gasita.",
                ["categories.saved"] = "Categoria a fost salvata cu succes.",
                ["categories.saveFailed"] = "Salvarea categoriei a esuat.",
                ["categories.addNew"] = "Adauga categorie noua",
                ["categories.addTitle"] = "Adauga categorie",
                ["categories.created"] = "Categoria a fost adaugata cu succes.",
                ["categories.createFailed"] = "Adaugarea categoriei a esuat.",
                ["orders.title"] = "Comenzi",
                ["orders.count"] = "comenzi",
                ["orders.loading"] = "Se incarca comenzile...",
                ["orders.empty"] = "Nu au fost gasite comenzi.",
                ["orders.search.placeholder"] = "Cauta dupa numarul comenzii",
                ["orders.filter.status"] = "Filtreaza dupa status",
                ["orders.details"] = "Detalii comanda",
                ["orders.loadingSingle"] = "Se incarca comanda...",
                ["orders.notFound"] = "Comanda nu a fost gasita.",
                ["orders.placedOn"] = "Plasata la",
                ["orders.changeStatus"] = "Schimba statusul",
                ["orders.customerInfo"] = "Informatii client",
                ["orders.deliveryAddress"] = "Adresa de livrare",
                ["orders.orderedProducts"] = "Produse comandate",
                ["orders.number"] = "Comanda",
                ["orders.updated"] = "Statusul a fost actualizat cu succes.",
                ["orders.updateFailed"] = "Actualizarea statusului a esuat.",
                ["orders.itemFallback"] = "Produs",
                ["orders.newSummaryTitle"] = "Comenzi noi",
                ["orders.newSummaryMessage"] = "Exista {0} comenzi cu status Nou.",
                ["orders.recentPlacedTitle"] = "Comanda noua plasata",
                ["orders.recentPlacedMessage"] = "Au fost plasate {0} comenzi noi in ultimele 2 minute.",
                ["orders.goToOrders"] = "Mergi la comenzi",
                ["status.new"] = "Noua",
                ["status.preparing"] = "In pregatire",
                ["status.delivered"] = "Livrata",
                ["status.active"] = "Activ",
                ["status.inactive"] = "Inactiv",
                ["login.portal"] = "Portal administrare",
                ["login.description"] = "Administreaza magazinul de materiale de constructii rapid si eficient.",
                ["login.feature.catalogue"] = "Administrare produse si categorii",
                ["login.feature.orders"] = "Urmarire comenzi si actualizare status",
                ["login.feature.security"] = "Acces securizat pe baza de roluri",
                ["login.welcome"] = "Bine ai revenit",
                ["login.subtitle"] = "Autentifica-te pentru a continua",
                ["login.email"] = "Adresa de email",
                ["login.emailPlaceholder"] = "Introdu adresa de email",
                ["login.password"] = "Parola",
                ["login.passwordPlaceholder"] = "Introdu parola",
                ["login.hidePassword"] = "Ascunde parola",
                ["login.showPassword"] = "Arata parola",
                ["login.signingIn"] = "Se autentifica...",
                ["login.signIn"] = "Autentificare",
                ["login.footer"] = "Administrare ConstructionStore",
                ["errors.invalidCredentials"] = "Email sau parola invalida.",
                ["errors.connection"] = "Nu se poate realiza conexiunea cu serverul. Verifica reteaua.",
                ["errors.unexpected"] = "A aparut o eroare neasteptata. Incearca din nou.",
                ["errors.loginFailed"] = "Autentificarea a esuat. Incearca din nou.",
                ["notFound.title"] = "Pagina nu a fost gasita",
                ["notFound.description"] = "Continutul cautat nu exista.",
                ["role.admin"] = "administrator",
                ["role.employee"] = "angajat"
            }
        };

    public string CurrentLanguage { get; private set; } = "ro";

    public CultureInfo CurrentCulture { get; private set; } = CreateCulture();

    public async Task InitializeAsync()
    {
        ApplyLanguage();
        await Task.CompletedTask;
    }

    public string T(string key)
    {
        if (Translations.TryGetValue(CurrentLanguage, out var languageMap) && languageMap.TryGetValue(key, out var value))
        {
            return value;
        }

        return key;
    }

    public string FormatPrice(decimal value) => $"{value.ToString("N2", CurrentCulture)} {T("common.currency")}";

    public string FormatPrice(decimal? value) => value.HasValue ? FormatPrice(value.Value) : "—";

    public string FormatDate(DateTime value, string format) => value.ToString(format, CurrentCulture);

    public string FormatChisinauDate(DateTime value, string format) => ConvertToChisinau(value).ToString(format, CurrentCulture);

    public string TranslateOrderStatus(string? status)
    {
        return NormalizeStatus(status) switch
        {
            "NOU" => T("status.new"),
            "PREPARING" => T("status.preparing"),
            "DELIVERED" => T("status.delivered"),
            _ => status ?? string.Empty
        };
    }

    public string TranslateActiveState(bool isActive) => isActive ? T("status.active") : T("status.inactive");

    public string TranslateRole(string? role)
    {
        return role?.Trim().ToLowerInvariant() switch
        {
            "admin" => T("role.admin"),
            "employee" => T("role.employee"),
            _ => role ?? string.Empty
        };
    }

    public string ProductLabel(int id) => $"{T("orders.itemFallback")} #{id}";

    public IReadOnlyList<(string Value, string Label)> GetOrderStatuses() =>
    [
        ("NOU", T("status.new")),
        ("PREPARING", T("status.preparing")),
        ("DELIVERED", T("status.delivered"))
    ];

    private void ApplyLanguage()
    {
        CurrentLanguage = "ro";
        CurrentCulture = CreateCulture();
        CultureInfo.DefaultThreadCurrentCulture = CurrentCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CurrentCulture;
    }

    private static CultureInfo CreateCulture() => new("ro-RO");

    private static TimeZoneInfo ResolveChisinauTimeZone()
    {
        foreach (var timeZoneId in new[] { "Europe/Chisinau", "E. Europe Standard Time", "GTB Standard Time" })
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            }
            catch (TimeZoneNotFoundException)
            {
            }
            catch (InvalidTimeZoneException)
            {
            }
        }

        return TimeZoneInfo.Local;
    }

    private static DateTime ConvertToChisinau(DateTime value)
    {
        if (value.Kind == DateTimeKind.Unspecified)
        {
            return value;
        }

        var utcValue = value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => value
        };

        return TimeZoneInfo.ConvertTimeFromUtc(utcValue, ChisinauTimeZone);
    }

    private static string NormalizeStatus(string? status)
    {
        var normalized = status?.Trim().ToUpperInvariant();
        return normalized == "NEW" ? "NOU" : normalized ?? string.Empty;
    }
}