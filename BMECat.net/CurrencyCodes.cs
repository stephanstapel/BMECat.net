/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 * 
 *   http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
using System;

namespace BMECat.net
{
    /// <summary>
    /// Currency codes based on ISO 4217
    ///
    /// source:
    /// https://www.six-group.com/dam/download/financial-information/data-center/iso-currrency/lists/list-one.xls
    ///
    /// Converted by filtering official currencies and ignoring funds,
    /// then grouping by code
    /// </summary>
    public enum CurrencyCodes
    {
        /// <summary>
        /// Afghani (AFN)
        /// Used in:
        /// - AFGHANISTAN
        /// </summary>
        AFN = 971,

        /// <summary>
        /// Euro (EUR)
        /// Used in:
        /// - ÅLAND ISLANDS
        /// - ANDORRA
        /// - AUSTRIA
        /// - BELGIUM
        /// - CROATIA
        /// - CYPRUS
        /// - ESTONIA
        /// - EUROPEAN UNION
        /// - FINLAND
        /// - FRANCE
        /// - FRENCH GUIANA
        /// - FRENCH SOUTHERN TERRITORIES (THE)
        /// - GERMANY
        /// - GREECE
        /// - GUADELOUPE
        /// - HOLY SEE (THE)
        /// - IRELAND
        /// - ITALY
        /// - LATVIA
        /// - LITHUANIA
        /// - LUXEMBOURG
        /// - MALTA
        /// - MARTINIQUE
        /// - MAYOTTE
        /// - MONACO
        /// - MONTENEGRO
        /// - NETHERLANDS (THE)
        /// - PORTUGAL
        /// - RÉUNION
        /// - SAINT BARTHÉLEMY
        /// - SAINT MARTIN (FRENCH PART)
        /// - SAINT PIERRE AND MIQUELON
        /// - SAN MARINO
        /// - SLOVAKIA
        /// - SLOVENIA
        /// - SPAIN
        /// </summary>
        EUR = 978,

        /// <summary>
        /// Lek (ALL)
        /// Used in:
        /// - ALBANIA
        /// </summary>
        ALL = 8,

        /// <summary>
        /// Algerian Dinar (DZD)
        /// Used in:
        /// - ALGERIA
        /// </summary>
        DZD = 12,

        /// <summary>
        /// US Dollar (USD)
        /// Used in:
        /// - AMERICAN SAMOA
        /// - BONAIRE, SINT EUSTATIUS AND SABA
        /// - BRITISH INDIAN OCEAN TERRITORY (THE)
        /// - ECUADOR
        /// - EL SALVADOR
        /// - GUAM
        /// - HAITI
        /// - MARSHALL ISLANDS (THE)
        /// - MICRONESIA (FEDERATED STATES OF)
        /// - NORTHERN MARIANA ISLANDS (THE)
        /// - PALAU
        /// - PANAMA
        /// - PUERTO RICO
        /// - TIMOR-LESTE
        /// - TURKS AND CAICOS ISLANDS (THE)
        /// - UNITED STATES MINOR OUTLYING ISLANDS (THE)
        /// - UNITED STATES OF AMERICA (THE)
        /// - VIRGIN ISLANDS (BRITISH)
        /// - VIRGIN ISLANDS (U.S.)
        /// </summary>
        USD = 840,

        /// <summary>
        /// Kwanza (AOA)
        /// Used in:
        /// - ANGOLA
        /// </summary>
        AOA = 973,

        /// <summary>
        /// East Caribbean Dollar (XCD)
        /// Used in:
        /// - ANGUILLA
        /// - ANTIGUA AND BARBUDA
        /// - DOMINICA
        /// - GRENADA
        /// - MONTSERRAT
        /// - SAINT KITTS AND NEVIS
        /// - SAINT LUCIA
        /// - SAINT VINCENT AND THE GRENADINES
        /// </summary>
        XCD = 951,

        /// <summary>
        /// Argentine Peso (ARS)
        /// Used in:
        /// - ARGENTINA
        /// </summary>
        ARS = 32,

        /// <summary>
        /// Armenian Dram (AMD)
        /// Used in:
        /// - ARMENIA
        /// </summary>
        AMD = 51,

        /// <summary>
        /// Aruban Florin (AWG)
        /// Used in:
        /// - ARUBA
        /// </summary>
        AWG = 533,

        /// <summary>
        /// Australian Dollar (AUD)
        /// Used in:
        /// - AUSTRALIA
        /// - CHRISTMAS ISLAND
        /// - COCOS (KEELING) ISLANDS (THE)
        /// - HEARD ISLAND AND McDONALD ISLANDS
        /// - KIRIBATI
        /// - NAURU
        /// - NORFOLK ISLAND
        /// - TUVALU
        /// </summary>
        AUD = 36,

        /// <summary>
        /// Azerbaijan Manat (AZN)
        /// Used in:
        /// - AZERBAIJAN
        /// </summary>
        AZN = 944,

        /// <summary>
        /// Bahamian Dollar (BSD)
        /// Used in:
        /// - BAHAMAS (THE)
        /// </summary>
        BSD = 44,

        /// <summary>
        /// Bahraini Dinar (BHD)
        /// Used in:
        /// - BAHRAIN
        /// </summary>
        BHD = 48,

        /// <summary>
        /// Taka (BDT)
        /// Used in:
        /// - BANGLADESH
        /// </summary>
        BDT = 50,

        /// <summary>
        /// Barbados Dollar (BBD)
        /// Used in:
        /// - BARBADOS
        /// </summary>
        BBD = 52,

        /// <summary>
        /// Belarusian Ruble (BYN)
        /// Used in:
        /// - BELARUS
        /// </summary>
        BYN = 933,

        /// <summary>
        /// Belize Dollar (BZD)
        /// Used in:
        /// - BELIZE
        /// </summary>
        BZD = 84,

        /// <summary>
        /// CFA Franc BCEAO (XOF)
        /// Used in:
        /// - BENIN
        /// - BURKINA FASO
        /// - CÔTE D'IVOIRE
        /// - GUINEA-BISSAU
        /// - MALI
        /// - NIGER (THE)
        /// - SENEGAL
        /// - TOGO
        /// </summary>
        XOF = 952,

        /// <summary>
        /// Bermudian Dollar (BMD)
        /// Used in:
        /// - BERMUDA
        /// </summary>
        BMD = 60,

        /// <summary>
        /// Indian Rupee (INR)
        /// Used in:
        /// - BHUTAN
        /// - INDIA
        /// </summary>
        INR = 356,

        /// <summary>
        /// Ngultrum (BTN)
        /// Used in:
        /// - BHUTAN
        /// </summary>
        BTN = 64,

        /// <summary>
        /// Boliviano (BOB)
        /// Used in:
        /// - BOLIVIA (PLURINATIONAL STATE OF)
        /// </summary>
        BOB = 68,

        /// <summary>
        /// Convertible Mark (BAM)
        /// Used in:
        /// - BOSNIA AND HERZEGOVINA
        /// </summary>
        BAM = 977,

        /// <summary>
        /// Pula (BWP)
        /// Used in:
        /// - BOTSWANA
        /// </summary>
        BWP = 72,

        /// <summary>
        /// Norwegian Krone (NOK)
        /// Used in:
        /// - BOUVET ISLAND
        /// - NORWAY
        /// - SVALBARD AND JAN MAYEN
        /// </summary>
        NOK = 578,

        /// <summary>
        /// Brazilian Real (BRL)
        /// Used in:
        /// - BRAZIL
        /// </summary>
        BRL = 986,

        /// <summary>
        /// Brunei Dollar (BND)
        /// Used in:
        /// - BRUNEI DARUSSALAM
        /// </summary>
        BND = 96,

        /// <summary>
        /// Bulgarian Lev (BGN)
        /// Used in:
        /// - BULGARIA
        /// </summary>
        BGN = 975,

        /// <summary>
        /// Burundi Franc (BIF)
        /// Used in:
        /// - BURUNDI
        /// </summary>
        BIF = 108,

        /// <summary>
        /// Cabo Verde Escudo (CVE)
        /// Used in:
        /// - CABO VERDE
        /// </summary>
        CVE = 132,

        /// <summary>
        /// Riel (KHR)
        /// Used in:
        /// - CAMBODIA
        /// </summary>
        KHR = 116,

        /// <summary>
        /// CFA Franc BEAC (XAF)
        /// Used in:
        /// - CAMEROON
        /// - CENTRAL AFRICAN REPUBLIC (THE)
        /// - CHAD
        /// - CONGO (THE)
        /// - EQUATORIAL GUINEA
        /// - GABON
        /// </summary>
        XAF = 950,

        /// <summary>
        /// Canadian Dollar (CAD)
        /// Used in:
        /// - CANADA
        /// </summary>
        CAD = 124,

        /// <summary>
        /// Cayman Islands Dollar (KYD)
        /// Used in:
        /// - CAYMAN ISLANDS (THE)
        /// </summary>
        KYD = 136,

        /// <summary>
        /// Chilean Peso (CLP)
        /// Used in:
        /// - CHILE
        /// </summary>
        CLP = 152,

        /// <summary>
        /// Yuan Renminbi (CNY)
        /// Used in:
        /// - CHINA
        /// </summary>
        CNY = 156,

        /// <summary>
        /// Colombian Peso (COP)
        /// Used in:
        /// - COLOMBIA
        /// </summary>
        COP = 170,

        /// <summary>
        /// Comorian Franc  (KMF)
        /// Used in:
        /// - COMOROS (THE)
        /// </summary>
        KMF = 174,

        /// <summary>
        /// Congolese Franc (CDF)
        /// Used in:
        /// - CONGO (THE DEMOCRATIC REPUBLIC OF THE)
        /// </summary>
        CDF = 976,

        /// <summary>
        /// New Zealand Dollar (NZD)
        /// Used in:
        /// - COOK ISLANDS (THE)
        /// - NEW ZEALAND
        /// - NIUE
        /// - PITCAIRN
        /// - TOKELAU
        /// </summary>
        NZD = 554,

        /// <summary>
        /// Costa Rican Colon (CRC)
        /// Used in:
        /// - COSTA RICA
        /// </summary>
        CRC = 188,

        /// <summary>
        /// Cuban Peso (CUP)
        /// Used in:
        /// - CUBA
        /// </summary>
        CUP = 192,

        /// <summary>
        /// Netherlands Antillean Guilder (ANG)
        /// Used in:
        /// - CURAÇAO
        /// - SINT MAARTEN (DUTCH PART)
        /// </summary>
        ANG = 532,

        /// <summary>
        /// Czech Koruna (CZK)
        /// Used in:
        /// - CZECHIA
        /// </summary>
        CZK = 203,

        /// <summary>
        /// Danish Krone (DKK)
        /// Used in:
        /// - DENMARK
        /// - FAROE ISLANDS (THE)
        /// - GREENLAND
        /// </summary>
        DKK = 208,

        /// <summary>
        /// Djibouti Franc (DJF)
        /// Used in:
        /// - DJIBOUTI
        /// </summary>
        DJF = 262,

        /// <summary>
        /// Dominican Peso (DOP)
        /// Used in:
        /// - DOMINICAN REPUBLIC (THE)
        /// </summary>
        DOP = 214,

        /// <summary>
        /// Egyptian Pound (EGP)
        /// Used in:
        /// - EGYPT
        /// </summary>
        EGP = 818,

        /// <summary>
        /// El Salvador Colon (SVC)
        /// Used in:
        /// - EL SALVADOR
        /// </summary>
        SVC = 222,

        /// <summary>
        /// Nakfa (ERN)
        /// Used in:
        /// - ERITREA
        /// </summary>
        ERN = 232,

        /// <summary>
        /// Lilangeni (SZL)
        /// Used in:
        /// - ESWATINI
        /// </summary>
        SZL = 748,

        /// <summary>
        /// Ethiopian Birr (ETB)
        /// Used in:
        /// - ETHIOPIA
        /// </summary>
        ETB = 230,

        /// <summary>
        /// Falkland Islands Pound (FKP)
        /// Used in:
        /// - FALKLAND ISLANDS (THE) [MALVINAS]
        /// </summary>
        FKP = 238,

        /// <summary>
        /// Fiji Dollar (FJD)
        /// Used in:
        /// - FIJI
        /// </summary>
        FJD = 242,

        /// <summary>
        /// CFP Franc (XPF)
        /// Used in:
        /// - FRENCH POLYNESIA
        /// - NEW CALEDONIA
        /// - WALLIS AND FUTUNA
        /// </summary>
        XPF = 953,

        /// <summary>
        /// Dalasi (GMD)
        /// Used in:
        /// - GAMBIA (THE)
        /// </summary>
        GMD = 270,

        /// <summary>
        /// Lari (GEL)
        /// Used in:
        /// - GEORGIA
        /// </summary>
        GEL = 981,

        /// <summary>
        /// Ghana Cedi (GHS)
        /// Used in:
        /// - GHANA
        /// </summary>
        GHS = 936,

        /// <summary>
        /// Gibraltar Pound (GIP)
        /// Used in:
        /// - GIBRALTAR
        /// </summary>
        GIP = 292,

        /// <summary>
        /// Quetzal (GTQ)
        /// Used in:
        /// - GUATEMALA
        /// </summary>
        GTQ = 320,

        /// <summary>
        /// Pound Sterling (GBP)
        /// Used in:
        /// - GUERNSEY
        /// - ISLE OF MAN
        /// - JERSEY
        /// - UNITED KINGDOM OF GREAT BRITAIN AND NORTHERN IRELAND (THE)
        /// </summary>
        GBP = 826,

        /// <summary>
        /// Guinean Franc (GNF)
        /// Used in:
        /// - GUINEA
        /// </summary>
        GNF = 324,

        /// <summary>
        /// Guyana Dollar (GYD)
        /// Used in:
        /// - GUYANA
        /// </summary>
        GYD = 328,

        /// <summary>
        /// Gourde (HTG)
        /// Used in:
        /// - HAITI
        /// </summary>
        HTG = 332,

        /// <summary>
        /// Lempira (HNL)
        /// Used in:
        /// - HONDURAS
        /// </summary>
        HNL = 340,

        /// <summary>
        /// Hong Kong Dollar (HKD)
        /// Used in:
        /// - HONG KONG
        /// </summary>
        HKD = 344,

        /// <summary>
        /// Forint (HUF)
        /// Used in:
        /// - HUNGARY
        /// </summary>
        HUF = 348,

        /// <summary>
        /// Iceland Krona (ISK)
        /// Used in:
        /// - ICELAND
        /// </summary>
        ISK = 352,

        /// <summary>
        /// Rupiah (IDR)
        /// Used in:
        /// - INDONESIA
        /// </summary>
        IDR = 360,

        /// <summary>
        /// Iranian Rial (IRR)
        /// Used in:
        /// - IRAN (ISLAMIC REPUBLIC OF)
        /// </summary>
        IRR = 364,

        /// <summary>
        /// Iraqi Dinar (IQD)
        /// Used in:
        /// - IRAQ
        /// </summary>
        IQD = 368,

        /// <summary>
        /// New Israeli Sheqel (ILS)
        /// Used in:
        /// - ISRAEL
        /// </summary>
        ILS = 376,

        /// <summary>
        /// Jamaican Dollar (JMD)
        /// Used in:
        /// - JAMAICA
        /// </summary>
        JMD = 388,

        /// <summary>
        /// Yen (JPY)
        /// Used in:
        /// - JAPAN
        /// </summary>
        JPY = 392,

        /// <summary>
        /// Jordanian Dinar (JOD)
        /// Used in:
        /// - JORDAN
        /// </summary>
        JOD = 400,

        /// <summary>
        /// Tenge (KZT)
        /// Used in:
        /// - KAZAKHSTAN
        /// </summary>
        KZT = 398,

        /// <summary>
        /// Kenyan Shilling (KES)
        /// Used in:
        /// - KENYA
        /// </summary>
        KES = 404,

        /// <summary>
        /// North Korean Won (KPW)
        /// Used in:
        /// - KOREA (THE DEMOCRATIC PEOPLE’S REPUBLIC OF)
        /// </summary>
        KPW = 408,

        /// <summary>
        /// Won (KRW)
        /// Used in:
        /// - KOREA (THE REPUBLIC OF)
        /// </summary>
        KRW = 410,

        /// <summary>
        /// Kuwaiti Dinar (KWD)
        /// Used in:
        /// - KUWAIT
        /// </summary>
        KWD = 414,

        /// <summary>
        /// Som (KGS)
        /// Used in:
        /// - KYRGYZSTAN
        /// </summary>
        KGS = 417,

        /// <summary>
        /// Lao Kip (LAK)
        /// Used in:
        /// - LAO PEOPLE’S DEMOCRATIC REPUBLIC (THE)
        /// </summary>
        LAK = 418,

        /// <summary>
        /// Lebanese Pound (LBP)
        /// Used in:
        /// - LEBANON
        /// </summary>
        LBP = 422,

        /// <summary>
        /// Loti (LSL)
        /// Used in:
        /// - LESOTHO
        /// </summary>
        LSL = 426,

        /// <summary>
        /// Rand (ZAR)
        /// Used in:
        /// - LESOTHO
        /// - NAMIBIA
        /// - SOUTH AFRICA
        /// </summary>
        ZAR = 710,

        /// <summary>
        /// Liberian Dollar (LRD)
        /// Used in:
        /// - LIBERIA
        /// </summary>
        LRD = 430,

        /// <summary>
        /// Libyan Dinar (LYD)
        /// Used in:
        /// - LIBYA
        /// </summary>
        LYD = 434,

        /// <summary>
        /// Swiss Franc (CHF)
        /// Used in:
        /// - LIECHTENSTEIN
        /// - SWITZERLAND
        /// </summary>
        CHF = 756,

        /// <summary>
        /// Pataca (MOP)
        /// Used in:
        /// - MACAO
        /// </summary>
        MOP = 446,

        /// <summary>
        /// Denar (MKD)
        /// Used in:
        /// - NORTH MACEDONIA
        /// </summary>
        MKD = 807,

        /// <summary>
        /// Malagasy Ariary (MGA)
        /// Used in:
        /// - MADAGASCAR
        /// </summary>
        MGA = 969,

        /// <summary>
        /// Malawi Kwacha (MWK)
        /// Used in:
        /// - MALAWI
        /// </summary>
        MWK = 454,

        /// <summary>
        /// Malaysian Ringgit (MYR)
        /// Used in:
        /// - MALAYSIA
        /// </summary>
        MYR = 458,

        /// <summary>
        /// Rufiyaa (MVR)
        /// Used in:
        /// - MALDIVES
        /// </summary>
        MVR = 462,

        /// <summary>
        /// Ouguiya (MRU)
        /// Used in:
        /// - MAURITANIA
        /// </summary>
        MRU = 929,

        /// <summary>
        /// Mauritius Rupee (MUR)
        /// Used in:
        /// - MAURITIUS
        /// </summary>
        MUR = 480,

        /// <summary>
        /// Mexican Peso (MXN)
        /// Used in:
        /// - MEXICO
        /// </summary>
        MXN = 484,

        /// <summary>
        /// Moldovan Leu (MDL)
        /// Used in:
        /// - MOLDOVA (THE REPUBLIC OF)
        /// </summary>
        MDL = 498,

        /// <summary>
        /// Tugrik (MNT)
        /// Used in:
        /// - MONGOLIA
        /// </summary>
        MNT = 496,

        /// <summary>
        /// Moroccan Dirham (MAD)
        /// Used in:
        /// - MOROCCO
        /// - WESTERN SAHARA
        /// </summary>
        MAD = 504,

        /// <summary>
        /// Mozambique Metical (MZN)
        /// Used in:
        /// - MOZAMBIQUE
        /// </summary>
        MZN = 943,

        /// <summary>
        /// Kyat (MMK)
        /// Used in:
        /// - MYANMAR
        /// </summary>
        MMK = 104,

        /// <summary>
        /// Namibia Dollar (NAD)
        /// Used in:
        /// - NAMIBIA
        /// </summary>
        NAD = 516,

        /// <summary>
        /// Nepalese Rupee (NPR)
        /// Used in:
        /// - NEPAL
        /// </summary>
        NPR = 524,

        /// <summary>
        /// Cordoba Oro (NIO)
        /// Used in:
        /// - NICARAGUA
        /// </summary>
        NIO = 558,

        /// <summary>
        /// Naira (NGN)
        /// Used in:
        /// - NIGERIA
        /// </summary>
        NGN = 566,

        /// <summary>
        /// Rial Omani (OMR)
        /// Used in:
        /// - OMAN
        /// </summary>
        OMR = 512,

        /// <summary>
        /// Pakistan Rupee (PKR)
        /// Used in:
        /// - PAKISTAN
        /// </summary>
        PKR = 586,

        /// <summary>
        /// Balboa (PAB)
        /// Used in:
        /// - PANAMA
        /// </summary>
        PAB = 590,

        /// <summary>
        /// Kina (PGK)
        /// Used in:
        /// - PAPUA NEW GUINEA
        /// </summary>
        PGK = 598,

        /// <summary>
        /// Guarani (PYG)
        /// Used in:
        /// - PARAGUAY
        /// </summary>
        PYG = 600,

        /// <summary>
        /// Sol (PEN)
        /// Used in:
        /// - PERU
        /// </summary>
        PEN = 604,

        /// <summary>
        /// Philippine Peso (PHP)
        /// Used in:
        /// - PHILIPPINES (THE)
        /// </summary>
        PHP = 608,

        /// <summary>
        /// Zloty (PLN)
        /// Used in:
        /// - POLAND
        /// </summary>
        PLN = 985,

        /// <summary>
        /// Qatari Rial (QAR)
        /// Used in:
        /// - QATAR
        /// </summary>
        QAR = 634,

        /// <summary>
        /// Romanian Leu (RON)
        /// Used in:
        /// - ROMANIA
        /// </summary>
        RON = 946,

        /// <summary>
        /// Russian Ruble (RUB)
        /// Used in:
        /// - RUSSIAN FEDERATION (THE)
        /// </summary>
        RUB = 643,

        /// <summary>
        /// Rwanda Franc (RWF)
        /// Used in:
        /// - RWANDA
        /// </summary>
        RWF = 646,

        /// <summary>
        /// Saint Helena Pound (SHP)
        /// Used in:
        /// - SAINT HELENA, ASCENSION AND TRISTAN DA CUNHA
        /// </summary>
        SHP = 654,

        /// <summary>
        /// Tala (WST)
        /// Used in:
        /// - SAMOA
        /// </summary>
        WST = 882,

        /// <summary>
        /// Dobra (STN)
        /// Used in:
        /// - SAO TOME AND PRINCIPE
        /// </summary>
        STN = 930,

        /// <summary>
        /// Saudi Riyal (SAR)
        /// Used in:
        /// - SAUDI ARABIA
        /// </summary>
        SAR = 682,

        /// <summary>
        /// Serbian Dinar (RSD)
        /// Used in:
        /// - SERBIA
        /// </summary>
        RSD = 941,

        /// <summary>
        /// Seychelles Rupee (SCR)
        /// Used in:
        /// - SEYCHELLES
        /// </summary>
        SCR = 690,

        /// <summary>
        /// Leone (SLE)
        /// Used in:
        /// - SIERRA LEONE
        /// </summary>
        SLE = 925,

        /// <summary>
        /// Singapore Dollar (SGD)
        /// Used in:
        /// - SINGAPORE
        /// </summary>
        SGD = 702,

        /// <summary>
        /// Solomon Islands Dollar (SBD)
        /// Used in:
        /// - SOLOMON ISLANDS
        /// </summary>
        SBD = 90,

        /// <summary>
        /// Somali Shilling (SOS)
        /// Used in:
        /// - SOMALIA
        /// </summary>
        SOS = 706,

        /// <summary>
        /// South Sudanese Pound (SSP)
        /// Used in:
        /// - SOUTH SUDAN
        /// </summary>
        SSP = 728,

        /// <summary>
        /// Sri Lanka Rupee (LKR)
        /// Used in:
        /// - SRI LANKA
        /// </summary>
        LKR = 144,

        /// <summary>
        /// Sudanese Pound (SDG)
        /// Used in:
        /// - SUDAN (THE)
        /// </summary>
        SDG = 938,

        /// <summary>
        /// Surinam Dollar (SRD)
        /// Used in:
        /// - SURINAME
        /// </summary>
        SRD = 968,

        /// <summary>
        /// Swedish Krona (SEK)
        /// Used in:
        /// - SWEDEN
        /// </summary>
        SEK = 752,

        /// <summary>
        /// Syrian Pound (SYP)
        /// Used in:
        /// - SYRIAN ARAB REPUBLIC
        /// </summary>
        SYP = 760,

        /// <summary>
        /// New Taiwan Dollar (TWD)
        /// Used in:
        /// - TAIWAN (PROVINCE OF CHINA)
        /// </summary>
        TWD = 901,

        /// <summary>
        /// Somoni (TJS)
        /// Used in:
        /// - TAJIKISTAN
        /// </summary>
        TJS = 972,

        /// <summary>
        /// Tanzanian Shilling (TZS)
        /// Used in:
        /// - TANZANIA, UNITED REPUBLIC OF
        /// </summary>
        TZS = 834,

        /// <summary>
        /// Baht (THB)
        /// Used in:
        /// - THAILAND
        /// </summary>
        THB = 764,

        /// <summary>
        /// Pa’anga (TOP)
        /// Used in:
        /// - TONGA
        /// </summary>
        TOP = 776,

        /// <summary>
        /// Trinidad and Tobago Dollar (TTD)
        /// Used in:
        /// - TRINIDAD AND TOBAGO
        /// </summary>
        TTD = 780,

        /// <summary>
        /// Tunisian Dinar (TND)
        /// Used in:
        /// - TUNISIA
        /// </summary>
        TND = 788,

        /// <summary>
        /// Turkish Lira (TRY)
        /// Used in:
        /// - TÜRKİYE
        /// </summary>
        TRY = 949,

        /// <summary>
        /// Turkmenistan New Manat (TMT)
        /// Used in:
        /// - TURKMENISTAN
        /// </summary>
        TMT = 934,

        /// <summary>
        /// Uganda Shilling (UGX)
        /// Used in:
        /// - UGANDA
        /// </summary>
        UGX = 800,

        /// <summary>
        /// Hryvnia (UAH)
        /// Used in:
        /// - UKRAINE
        /// </summary>
        UAH = 980,

        /// <summary>
        /// UAE Dirham (AED)
        /// Used in:
        /// - UNITED ARAB EMIRATES (THE)
        /// </summary>
        AED = 784,

        /// <summary>
        /// Peso Uruguayo (UYU)
        /// Used in:
        /// - URUGUAY
        /// </summary>
        UYU = 858,

        /// <summary>
        /// Unidad Previsional (UYW)
        /// Used in:
        /// - URUGUAY
        /// </summary>
        UYW = 927,

        /// <summary>
        /// Uzbekistan Sum (UZS)
        /// Used in:
        /// - UZBEKISTAN
        /// </summary>
        UZS = 860,

        /// <summary>
        /// Vatu (VUV)
        /// Used in:
        /// - VANUATU
        /// </summary>
        VUV = 548,

        /// <summary>
        /// Bolívar Soberano (VES)
        /// Used in:
        /// - VENEZUELA (BOLIVARIAN REPUBLIC OF)
        /// </summary>
        VES = 928,

        /// <summary>
        /// Bolívar Soberano (VED)
        /// Used in:
        /// - VENEZUELA (BOLIVARIAN REPUBLIC OF)
        /// </summary>
        VED = 926,

        /// <summary>
        /// Dong (VND)
        /// Used in:
        /// - VIET NAM
        /// </summary>
        VND = 704,

        /// <summary>
        /// Yemeni Rial (YER)
        /// Used in:
        /// - YEMEN
        /// </summary>
        YER = 886,

        /// <summary>
        /// Zambian Kwacha (ZMW)
        /// Used in:
        /// - ZAMBIA
        /// </summary>
        ZMW = 967,

        /// <summary>
        /// Zimbabwe Gold (ZWG)
        /// Used in:
        /// - ZIMBABWE
        /// </summary>
        ZWG = 924,

        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0,
    }



    internal static class CurrencyCodesExtensions
    {
        public static CurrencyCodes FromString(this CurrencyCodes _c, string s)
        {
            try
            {
                return (CurrencyCodes)Enum.Parse(typeof(CurrencyCodes), s);
            }
            catch
            {
                return CurrencyCodes.Unknown;
            }
        } // !FromString()


        public static string EnumToString(this CurrencyCodes c)
        {
            return c.ToString("g");
        } // !ToString()
    }
}
