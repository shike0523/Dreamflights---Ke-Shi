function TranslateLanguage(originalLang, targetLang) {
    if ($("#languageSelect").val() === "zh") {
        return targetLang;
    } else {
        return originalLang;
    }
}