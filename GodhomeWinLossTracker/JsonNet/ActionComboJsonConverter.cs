﻿//#nullable enable

//using System;
//using Newtonsoft.Json;
//using GodhomeWinLossTracker.Input;

//namespace GodhomeWinLossTracker.JsonNet
//{
//    internal class ActionComboJsonConverter : JsonConverter
//    {
//        public override bool CanConvert(Type objectType)
//        {
//            return objectType == typeof(ActionCombo);
//        }

//        public override object? ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
//        {
//            if (objectType != typeof(ActionCombo))
//                throw new JsonException("Object is not ActionCombo.");

//            return Keybinds.KeybindStringToCombo(reader.Value as string);
//        }

//        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
//        {
//            if (value is not ActionCombo actionCombo)
//                throw new JsonException("Object is not ActionCombo.");

//            writer.WriteValue(Keybinds.ComboToKeybindString(actionCombo));
//        }
//    }
//}