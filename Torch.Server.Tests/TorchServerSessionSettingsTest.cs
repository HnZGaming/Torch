﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Torch.Server.ViewModels;
using VRage.Game;
using Xunit;

namespace Torch.Server.Tests
{
    public class TorchServerSessionSettingsTest
    {
        public static PropertyInfo[] ViewModelProperties = typeof(SessionSettingsViewModel).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        public static IEnumerable<object[]> ModelFields = typeof(MyObjectBuilder_SessionSettings).GetFields(BindingFlags.Public | BindingFlags.Instance).Select(x => new object[] { x });

        [Theory]
        [MemberData(nameof(ModelFields))]
        public void MissingPropertyTest(FieldInfo modelField)
        {
            var match = ViewModelProperties.FirstOrDefault(p => p.Name.Equals(modelField.Name, StringComparison.InvariantCultureIgnoreCase));
            Assert.NotNull(match);
        }
    }
}
