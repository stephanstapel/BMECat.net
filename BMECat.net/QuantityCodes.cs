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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace BMECat.net
{
    /// <summary>
    /// UN/ECE Recommendation 20
    /// Package Units und ausgewählte physikalische Einheiten
    /// 
    /// </summary>
    public enum QuantityCodes
    {
        Unknown,
        Code_04,
        Code_05,
        Code_08,
        Code_10,
        Code_11,
        Code_13,
        Code_14,
        Code_15,
        Code_16,
        Code_17,
        Code_18,
        Code_19,
        Code_1A,
        Code_1B,
        Code_1C,
        Code_1D,
        Code_1E,
        Code_1F,
        Code_1G,
        Code_1H,
        Code_1I,
        Code_1J,
        Code_1K,
        Code_1L,
        Code_1M,
        Code_1X,
        Code_20,
        Code_21,
        Code_22,
        Code_23,
        Code_24,
        Code_25,
        Code_26,
        Code_27,

        /// <summary>
        /// English: kilogram per square metre
        /// </summary>
        Code_28,
        Code_29,
        Code_2A,

        /// <summary>
        /// English: radian per second squared
        /// </summary>
        Code_2B,
        Code_2C,
        Code_2I,
        Code_2J,
        Code_2K,
        Code_2L,

        /// <summary>
        /// English: centimetre per second
        /// </summary>
        Code_2M,

        /// <summary>
        /// English: decibel
        /// </summary>
        Code_2N,
        Code_2P,
        Code_2Q,
        Code_2R,
        Code_2U,
        Code_2V,
        Code_2W,

        /// <summary>
        /// English: metre per minute
        /// </summary>
        Code_2X,
        Code_2Y,
        Code_2Z,
        Code_30,
        Code_31,
        Code_32,
        Code_33,
        Code_34,
        Code_35,
        Code_36,
        Code_37,
        Code_38,
        Code_3B,
        Code_3C,
        Code_3E,
        Code_3G,
        Code_3H,
        Code_3I,
        Code_40,
        Code_41,
        Code_43,
        Code_44,
        Code_45,
        Code_46,
        Code_47,
        Code_48,
        Code_4A,
        Code_4B,
        Code_4C,
        Code_4E,

        /// <summary>
        /// English: microlitre
        /// </summary>
        Code_4G,

        /// <summary>
        /// English: micrometre (micron)
        /// </summary>
        Code_4H,

        /// <summary>
        /// English: milliampere
        /// </summary>
        Code_4K,
        Code_4L,
        Code_4M,
        Code_4N,
        Code_4O,
        Code_4P,
        Code_4Q,
        Code_4R,
        Code_4T,
        Code_4U,
        Code_4W,
        Code_4X,
        Code_53,
        Code_54,
        Code_56,
        Code_57,
        Code_58,
        Code_59,
        Code_5A,
        Code_5B,
        Code_5C,
        Code_5E,
        Code_5F,
        Code_5G,
        Code_5H,
        Code_5I,
        Code_5J,
        Code_5K,
        Code_5P,
        Code_5Q,
        Code_60,
        Code_61,
        Code_62,
        Code_63,
        Code_64,
        Code_66,
        Code_69,
        Code_71,
        Code_72,
        Code_73,
        Code_74,
        Code_76,
        Code_77,
        Code_78,
        Code_80,
        Code_81,
        Code_84,
        Code_85,
        Code_87,
        Code_89,
        Code_90,
        Code_91,
        Code_92,
        Code_93,
        Code_94,
        Code_95,
        Code_96,
        Code_97,
        Code_98,
        A1,
        A10,
        A11,
        A12,
        A13,
        A14,
        A15,
        A16,
        A17,
        A18,
        A19,
        A2,
        A20,
        A21,
        A22,
        A23,
        A24,
        A25,
        A26,
        A27,
        A28,
        A29,
        A3,
        A30,
        A31,
        A32,
        A33,
        A34,
        A35,
        A36,
        A37,
        A38,
        A39,
        A4,
        A40,
        A41,
        A42,
        A43,
        A44,
        A45,
        A47,
        A48,
        A49,
        A5,
        A50,
        A51,
        A52,
        A53,
        A54,
        A55,
        A56,
        A57,
        A58,
        A6,
        A60,
        A61,
        A62,
        A63,
        A64,
        A65,
        A66,
        A67,
        A68,
        A69,
        A7,
        A70,
        A71,
        A73,
        A74,
        A75,
        A76,
        A77,
        A78,
        A79,
        A8,
        A80,
        A81,
        A82,
        A83,
        A84,
        A85,
        A86,
        A87,
        A88,
        A89,
        A9,
        A90,
        A91,
        A93,
        A94,
        A95,
        A96,
        A97,
        A98,

        /// <summary>
        /// English: bit
        /// </summary>
        A99,

        /// <summary>
        /// English: Ball
        /// German: Knäuel
        /// </summary>
        AA,
        AB,
        ACR,

        /// <summary>
        /// byte
        /// </summary>
        AD,
        AE,
        AH,
        AI,
        AJ,
        AK,
        AL,
        AM,

        /// <summary>
        /// ampere hour
        /// </summary>
        AMH,

        /// <summary>
        /// Ampere
        /// </summary>
        AMP,

        /// <summary>
        /// English: year
        /// </summary>
        ANN,
        AP,
        APZ,
        AQ,
        AR,
        ARE,
        AS,
        ASM,
        ASU,
        ATM,
        ATT,
        AV,
        AW,
        AY,
        AZ,
        B0,
        B1,
        B11,
        B12,
        B13,
        B14,
        B15,
        B16,
        B18,
        B2,
        B20,

        /// <summary>
        /// English: kelvin per watt
        /// </summary>
        B21,
        B22,
        B23,
        B24,
        B25,
        B26,
        B27,
        B28,
        B29,
        B3,
        B31,
        B32,
        B33,
        B34,
        B35,
        B36,
        B37,
        B38,
        B39,
        B4,
        B40,
        B41,
        B42,
        B43,
        B44,
        B45,
        B46,

        /// <summary>
        /// English: kilonewton
        /// </summary>
        B47,
        B48,
        B49,
        B5,
        B50,
        B51,
        B52,
        B53,
        B54,
        B55,
        B56,
        B57,
        B58,
        B59,
        B6,
        B60,
        B61,
        B62,
        B63,
        B64,
        B65,
        B66,
        B67,
        B69,
        B7,
        B70,
        B71,
        B72,
        B73,
        B74,
        B75,
        B76,
        B77,
        B78,
        B79,
        B8,
        B81,
        B83,

        /// <summary>
        /// English: microampere
        /// </summary>
        B84,
        B85,
        B86,
        B87,
        B88,
        B89,
        B9,
        B90,
        B91,
        B92,
        B93,
        B94,
        B95,
        B96,
        B97,

        /// <summary>
        /// English: microsecond
        /// </summary>
        B98,
        B99,
        BAR,
        BB,
        BD,
        BE,
        BFT,

        /// <summary>
        /// English: Bag
        /// German: Beutel
        /// </summary>
        BG,
        BH,
        BHP,
        BIL,

        /// <summary>
        /// English: Bucket
        /// German: Eimer
        /// </summary>
        BJ,
        BK,

        /// <summary>
        /// English: Bale
        /// German: Ballen
        /// </summary>
        BL,
        BLD,
        BLL,

        /// <summary>
        /// English: Bottle
        /// German: Flasche
        /// </summary>
        BO,
        BP,

        /// <summary>
        /// English: beats per minute
        /// </summary>
        BPM,
        BQL,
        BR,
        BT,
        BTU,
        BUA,
        BUI,
        BW,

        /// <summary>
        /// English: Box
        /// German: Box
        /// </summary>
        BX,
        BZ,
        C0,
        C1,
        C10,
        C11,
        C12,
        C13,

        /// <summary>
        /// English: millihenry
        /// </summary>
        C14,
        C15,
        C16,

        /// <summary>
        /// Enlish: millimetre squared per second
        /// </summary>
        C17,
        C18,
        C19,
        C2,
        C20,
        C22,
        C23,
        C24,
        C25,

        /// <summary>
        /// English: millisecond
        /// </summary>
        C26,
        C27,
        C28,
        C29,
        C3,
        C30,
        C31,
        C32,
        C33,
        C34,
        C35,
        C36,
        C38,
        C39,
        C4,
        C40,

        /// <summary>
        /// English: nanofarad
        /// </summary>
        C41,
        C42,
        C43,
        C44,

        /// <summary>
        /// English: nanometre
        /// </summary>
        C45,
        C46,
        C47,
        C48,
        C49,
        C5,
        C50,
        C51,
        C52,
        C53,
        C54,
        C55,

        /// <summary>
        /// newton per square millimetre
        /// </summary>
        C56,
        C57,
        C58,
        C59,
        C6,
        C60,
        C61,

        /// <summary>
        /// English: Piece
        /// German: Stück
        /// </summary>
        C62,
        C63,
        C64,
        C65,
        C66,
        C67,
        C68,
        C69,
        C7,
        C70,
        C71,
        C72,
        C73,
        C75,
        C76,
        C77,
        C78,
        C8,
        C80,
        C81,
        C82,
        C83,
        C84,
        C85,
        C86,
        C87,
        C88,
        C89,
        C9,
        C90,
        C91,
        C92,
        C93,
        C94,
        C95,
        C96,
        C97,
        C98,
        C99,

        /// <summary>
        /// English: Can
        /// German: Dose/ Büchse
        /// </summary>
        CA,
        CCT,
        CDL,
        CEL,
        CEN,

        /// <summary>
        /// English: Card
        /// German: Karton
        /// </summary>
        CG,
        CGM,

        /// <summary>
        /// English: Container
        /// German: Container
        /// </summary>
        CH,

        /// <summary>
        /// English: jerrycan
        /// German: Kanister
        /// </summary>
        CI,
        CJ,
        CK,
        CKG,

        /// <summary>
        /// English: Coil
        /// German: Bund
        /// </summary>
        CL,
        CLF,
        CLT,
        CMK,

        /// <summary>
        /// English: cubic centimetre
        /// </summary>
        CMQ,

        /// <summary>
        /// English: Centimetre
        /// German: Zentimet
        /// </summary>
        CMT,
        CNP,
        CNT,
        CO,
        COU,
        CQ,
        CR,

        /// <summary>
        /// English: Case
        /// German: Etui
        /// </summary>
        CS,

        /// <summary>
        /// English: Carton
        /// German: Kartonage
        /// </summary>
        CT,
        CTM,
        CU,
        CUR,
        CV,
        CWA,
        CWI,
        CY,
        CZ,
        D1,
        D10,
        D12,
        D13,
        D14,
        D15,
        D16,
        D17,
        D18,
        D19,
        D2,
        D20,
        D21,
        D22,
        D23,
        D24,
        D25,
        D26,
        D27,
        D28,
        D29,
        D30,
        D31,
        D32,
        D33,
        D34,
        D35,
        D37,
        D38,
        D39,
        D40,
        D41,
        D42,
        D43,
        D44,
        D45,

        /// <summary>
        /// English: volt - ampere
        /// </summary>
        D46,
        D47,
        D48,
        D49,
        D5,
        D50,

        /// <summary>
        /// English: volt per millimetre
        /// </summary>
        D51,
        D52,
        D53,
        D54,
        D55,
        D56,
        D57,
        D58,
        D59,
        D6,
        D60,
        D61,
        D62,
        D63,

        /// <summary>
        /// English: Block
        /// German: Block
        /// </summary>
        D64,
        D65,
        D66,
        D67,
        D69,
        D7,
        D70,
        D71,
        D72,
        D73,
        D74,
        D75,
        D76,
        D77,
        D79,
        D8,
        D80,
        D81,
        D82,
        D83,
        D85,
        D86,
        D87,
        D88,
        D89,
        D9,
        D90,
        D91,
        D92,
        D93,
        D94,
        D95,

        /// <summary>
        /// English: Palett
        /// German: Palette
        /// </summary>
        D96,
        D97,
        D98,
        D99,
        DAA,
        DAD,

        /// <summary>
        /// Tag
        /// Abkürzung: Tag(e)
        /// </summary>
        DAY,
        DB,
        DC,

        /// <summary>
        /// English: degree [unit of angle]
        /// </summary>
        DD,
        DE,
        DEC,
        DG,

        /// <summary>
        /// English: Dispenser
        /// German: Bebhälter
        /// </summary>
        DI,
        DJ,
        DLT,
        DMK,

        /// <summary>
        /// English: cubic decimetre
        /// </summary>
        DMQ,
        DMT,
        DN,

        /// <summary>
        /// English: Dozen Piece
        /// German: 12 Stück
        /// </summary>
        DPC,
        DPR,
        DPT,
        DQ,
        DR,
        DRA,
        DRI,
        DRL,
        DRM,
        DS,
        DT,
        DTN,
        DU,
        DWT,
        DX,
        DY,

        /// <summary>
        /// English: Dozen
        /// German: Dutzend
        /// </summary>
        DZN,
        DZP,
        E10,
        E11,
        E2,
        E3,
        E4,
        E5,
        E20,

        /// <summary>
        /// English: litre per hour
        /// </summary>
        E32,

        /// <summary>
        /// English: pixel
        /// </summary>
        E37,

        /// <summary>
        /// English: kilogram per hour
        /// </summary>
        E93,

        /// <summary>
        /// English: Each
        /// German: Stück
        /// </summary>
        EA,
        EB,
        EC,
        EP,
        EQ,
        EV,
        F1,
        F9,

        /// <summary>
        /// English: kilogram square millimetre
        /// </summary>
        F19,
        F76,

        /// <summary>
        /// English: inch of mercury
        /// </summary>
        F79,

        /// <summary>
        /// English: newton centimetre
        /// </summary>
        F88,

        /// <summary>
        /// English: newton metre per ampere
        /// </summary>
        F90,
        FAH,
        FAR,
        FB,
        FC,
        FD,
        FE,
        FF,
        FG,
        FH,
        FL,
        FM,
        FOT,
        FP,
        FR,
        FS,
        FTK,
        FTQ,
        G2,
        G3,
        G7,
        GB,

        /// <summary>
        /// English: cubic centimetre per hour
        /// </summary>
        G48,

        /// <summary>
        /// English: litre per second
        /// </summary>
        G51,

        /// <summary>
        /// English: millimetre to the fourth power
        /// </summary>
        G77,

        /// <summary>
        /// English: litre per second bar
        /// </summary>
        G85,
        GBQ,
        GC,
        GD,
        GE,
        GF,
        GFI,
        GGR,
        GH,
        GIA,
        GII,
        GJ,
        GK,
        GL,
        GLD,
        GLI,
        GLL,
        GM,
        GN,
        GO,

        /// <summary>
        /// English: milligram per cubic metre
        /// </summary>
        GP,
        GQ,

        /// <summary>
        /// English: Gram
        /// </summary>
        GRM,

        /// <summary>
        /// English: Gram
        /// German: Gramm
        /// </summary>
        GRN,
        GRO,
        GRT,
        GT,
        GV,
        GW,
        GWH,
        GY,
        GZ,
        H1,
        H2,

        /// <summary>
        /// English: percent per kelvin
        /// </summary>
        H25,

        /// <summary>
        /// millivolt per minute
        /// </summary>
        H62,

        /// <summary>
        /// English: percent per bar
        /// </summary>
        H96,
        HA,

        /// <summary>
        /// Hektar
        /// Abkürzung: ha
        /// </summary>
        HAR,
        HBA,
        HBX,
        HC,
        HD,
        HE,
        HF,
        HGM,
        HH,
        HI,
        HIU,
        HJ,
        HK,
        HL,
        HLT,
        HM,
        HMQ,
        HMT,
        HN,
        HO,
        HP,
        HPA,
        HS,
        HT,
        HTZ,
        /// <summary>
        /// English: Hour
        /// German: Stunde
        /// Abkürzung: Std.
        /// </summary>
        HUR,
        HY,
        IA,
        IC,
        IE,
        IF,
        II,
        IL,
        IM,
        INH,
        INK,
        INQ,
        IP,
        IT,
        IU,
        IV,
        J2,

        /// <summary>
        /// English: watt second
        /// </summary>
        J55,
        JB,
        JE,
        JG,
        JK,
        JM,
        JO,

        /// <summary>
        /// Joule
        /// </summary>
        JOU,

        /// <summary>
        /// English: Jar
        /// German: Glas/ Gefäss
        /// </summary>
        JR,
        K1,
        K2,
        K3,
        K5,
        K6,
        KA,
        KB,
        KBA,
        KD,
        KEL,
        KF,
        KG,

        /// <summary>
        /// English: Kilogram
        /// German: Kilogramm
        /// </summary>
        KGM,
        KGS,
        KHZ,
        KI,
        KJ,
        KJO,

        /// <summary>
        /// English: kilogram per metre
        /// </summary>
        KL,

        /// <summary>
        /// English: kilometre per hour
        /// </summary>
        KMH,
        KMK,
        KMQ,

        /// <summary>
        /// English: kilometre
        /// </summary>
        KMT,
        KNI,
        KNS,
        KNT,
        KO,
        KPA,
        KPH,
        KPO,
        KPP,
        KR,
        KS,
        KSD,
        KSH,

        /// <summary>
        /// English: Kit
        /// German: Kit
        /// </summary>
        KT,

        /// <summary>
        /// Kilometer
        /// Abkürzung: km
        /// </summary>
        KTM,
        KTN,
        KUR,
        KVA,
        KVR,

        /// <summary>
        /// kilovolt
        /// </summary>
        KVT,
        KW,

        /// <summary>
        /// Kilowattstune
        /// Abkürzung: kWh
        /// </summary>
        KWH,
        KWT,
        KX,
        L2,

        /// <summary>
        /// English: psi per psi
        /// </summary>
        L52,
        LA,
        LBR,
        LBT,
        LC,
        LD,
        LE,
        LEF,
        LF,
        LH,
        LI,
        LJ,
        LK,
        LM,
        LN,
        LO,
        LP,
        LPA,
        LR,

        /// <summary>
        /// Pauschale
        /// Abkürzung: pausch.
        /// </summary>
        LS,
        LTN,

        /// <summary>
        /// English: Litre
        /// German: Liter
        /// </summary>
        LTR,

        /// <summary>
        /// English: lumen
        /// </summary>
        LUM,
        LUX,
        LX,
        LY,
        M0,
        M1,
        M4,
        M5,
        M7,
        M9,

        /// <summary>
        /// English: revolution per minute
        /// </summary>
        M46,

        /// <summary>
        /// English: newton metre per radian
        /// </summary>
        M93,
        MA,
        MAL,
        MAM,
        MAW,
        MBE,
        MBF,
        MBR,
        MC,
        MCU,
        MD,
        MF,
        MGM,

        /// <summary>
        /// English: megahertz
        /// </summary>
        MHZ,
        MIK,
        MIL,

        /// <summary>
        /// English: minute [unit of time]
        /// German: Minute
        /// Abkürzung: min
        /// </summary>
        MIN,
        MIO,
        MIU,
        MK,
        MLD,

        /// <summary>
        /// English: Mililitre
        /// German: Mililiter
        /// </summary>
        MLT,

        /// <summary>
        /// Quadratmillimeter
        /// Abkürzung: mm^2
        /// </summary>
        MMK,
        MMQ,

        /// <summary>
        /// English: Milimetre
        /// German: Milimeter
        /// </summary>
        MMT,
        MON,
        MPA,
        MQ,
        MQH,
        MQS,

        /// <summary>
        /// English: metre per second squared
        /// </summary>
        MSK,
        MT,

        /// <summary>
        /// English: Square Metre
        /// German: Quadratmeter
        /// </summary>
        MTK,

        /// <summary>
        /// Kubikmeter
        /// Abkürzung: m^3 
        /// </summary>
        MTQ,

        /// <summary>
        /// English: Metre
        /// German: Meter
        /// </summary>
        MTR,

        /// <summary>
        /// English: metre per second
        /// </summary>
        MTS,
        MV,
        MVA,
        MWH,
        N1,
        N2,
        N3,
        NA,

        /// <summary>
        /// Anzahl Artikel
        /// Abkürzung: Anz.
        /// </summary>
        NAR,
        NB,
        NBB,
        NC,
        NCL,
        ND,
        NE,

        /// <summary>
        /// English: Newton
        /// </summary>
        NEW,
        NF,
        NG,
        NH,
        NI,
        NIU,
        NJ,
        NL,
        NMI,
        NMP,
        NN,
        NPL,

        /// <summary>
        /// Anzahl Paare
        /// Abkürzung: Pr.
        /// </summary>
        NPR,
        NPT,
        NQ,
        NR,
        NRL,
        NT,
        NTT,
        NU,
        NV,
        NX,
        NY,
        OA,
        OHM,
        ON,
        ONZ,

        /// <summary>
        /// English: Two Pack
        /// German: 2er-Pack
        /// </summary>
        OP,
        OT,
        OZ,
        OZA,
        OZI,
        P0,

        /// <summary>
        /// English: Percent
        /// German: Prozent
        /// Abkürzung: %
        /// </summary>
        P1,
        P2,
        P3,
        P4,
        P5,
        P6,
        P7,
        P8,
        P9,

        /// <summary>
        /// English: Packet
        /// German: Päckchen
        /// </summary>
        PA,
        PAL,
        PB,

        /// <summary>
        /// Stück
        /// </summary>
        PCE,

        /// <summary>
        /// English: Pad
        /// German: Block
        /// </summary>
        PD,
        PE,

        /// <summary>
        /// English: Pallet
        /// German: Palette
        /// </summary>
        PF,
        PG,
        PGL,
        PI,

        /// <summary>
        /// English: Pack
        /// German: Paket
        /// </summary>
        PK,
        PL,
        PM,
        PN,
        PO,
        PQ,

        /// <summary>
        /// English: Pair
        /// German: Paar
        /// </summary>
        PR,
        PS,
        PT,
        PTD,
        PTI,
        PTL,
        PU,
        PV,
        PW,
        PY,
        PZ,
        Q3,
        QA,
        QAN,
        QB,
        QD,
        QH,
        QK,

        /// <summary>
        /// English: Quire
        /// German: Bogen
        /// </summary>
        QR,
        QT,
        QTD,
        QTI,
        QTL,
        QTR,
        R1,
        R4,
        R9,
        RA,
        RD,
        RG,
        RH,
        RK,

        /// <summary>
        /// English: Reel
        /// German: Spule
        /// </summary>
        RL,
        RM,
        RN,

        /// <summary>
        /// English: Roll
        /// German: Rolle
        /// </summary>
        RO,
        RP,

        /// <summary>
        /// English: revolutions per minute
        /// </summary>
        RPM,
        RPS,
        RS,
        RT,
        RU,
        S3,
        S4,
        S5,
        S6,
        S7,
        S8,

        /// <summary>
        /// English: Sack
        /// German: Sack
        /// </summary>
        SA,
        SAN,
        SCO,
        SCR,
        SD,
        SE,

        /// <summary>
        /// English: second [unit of time]
        /// </summary>
        SEC,

        /// <summary>
        /// English: Set
        /// German: Satz
        /// </summary>
        SET,
        SG,
        SHT,
        SIE,
        SK,
        SL,
        SMI,
        SN,
        SO,
        SP,
        SQ,
        SR,
        SS,
        SST,

        /// <summary>
        /// English: Sheet
        /// German: Blatt
        /// </summary>
        ST,
        STI,
        STN,
        SV,
        SW,
        SX,
        T0,
        T1,
        T3,
        T4,
        T5,
        T6,
        T7,
        T8,
        TA,
        TAH,
        TC,
        TD,
        TE,
        TF,
        TI,
        TJ,
        TK,
        TL,

        /// <summary>
        /// English: Tin
        /// German: Dose
        /// </summary>
        TN,

        /// <summary>
        /// Tonne (metrisch)
        /// Abkürzung:  t
        /// </summary>
        TNE,
        TP,
        TPR,
        TQ,
        TQD,
        TR,
        TRL,
        TS,
        TSD,
        TSH,
        TT,

        /// <summary>
        /// English: Tube
        /// German: Tube/ Rohr
        /// </summary>
        TU,
        TV,
        TW,
        TY,
        U1,
        U2,
        UA,
        UB,
        UC,
        UD,
        UE,
        UF,
        UH,
        UM,
        VA,
        VI,

        /// <summary>
        /// English: Volt
        /// </summary>
        VLT,
        VQ,
        VS,
        W2,
        W4,
        WA,
        WB,
        WCD,
        WE,
        WEB,

        /// <summary>
        /// Woche
        /// Abkürzung: Woche(n)
        /// </summary>
        WEE,
        WG,
        WH,
        WHR,
        WI,
        WM,
        WR,
        WSD,

        /// <summary>
        /// Watt
        /// </summary>
        WTT,
        WW,
        X1,
        YDK,
        YDQ,
        YL,
        YRD,
        YT,
        Z1,
        Z2,

        /// <summary>
        /// English: Cask
        /// German: Fass
        /// </summary>
        Z3,
        Z4,
        Z5,
        Z6,
        Z8,
        ZP,
        ZZ,
    }


    public static class QuantityCodesExtensions
    {
        /// <summary>
        /// As the performance of parsing enums with a large enum value set is really slow, we create
        /// a helper dictionary to speed up the lookup
        /// </summary>
        private static Dictionary<string, QuantityCodes> _Mapping = null;
        private static ReaderWriterLockSlim _Lock = new ReaderWriterLockSlim();


        public static QuantityCodes FromString(this QuantityCodes _c, string s)
        {
            _Lock.EnterWriteLock();
            if (_Mapping == null)
            {             
                _fillMapping();                
            }
            _Lock.ExitWriteLock();

            try
            {
                _Lock.EnterReadLock();
                QuantityCodes retval = QuantityCodes.Unknown;
                if (_Mapping.ContainsKey(s))
                {
                    retval = _Mapping[s];
                }                
                return retval;
            }
            catch
            {
                return QuantityCodes.Unknown;
            }
            finally
            {
                _Lock.ExitReadLock();
            }
        } // !FromString()


        public static string EnumToString(this QuantityCodes c)
        {
            return c.ToString("g");
        } // !ToString()


        private static void _fillMapping()
        {
            _Mapping = new Dictionary<string, QuantityCodes>();
            foreach(QuantityCodes code in Enum.GetValues(typeof(QuantityCodes)))
            {
                _Mapping.Add(code.ToString(), code);
            }
        } // !_fillMapping()
    }
}
