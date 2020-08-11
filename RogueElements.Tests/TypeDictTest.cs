// <copyright file="TypeDictTest.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace RogueElements.Tests
{
    [TestFixture]
    public class TypeDictTest
    {
        private TypeDict<TestBaseClass> typeDict;

        [SetUp]
        public void TypeDictSetUp()
        {
            this.typeDict = new TypeDict<TestBaseClass>();
            this.typeDict.Set(new TestChildClassA());
            this.typeDict.Set(new TestChildClassB());
        }

        [Test]
        public void SetRepeat()
        {
            TestChildClassA testClass = new TestChildClassA
            {
                Alpha = "test",
            };
            this.typeDict.Set(testClass);
            Assert.That(this.typeDict.Get<TestChildClassA>(), Is.EqualTo(testClass));
        }

        [Test]
        public void SetNull()
        {
            Assert.Throws<ArgumentNullException>(() => { this.typeDict.Set(null); });
        }

        [Test]
        public void InterfaceSet()
        {
            TestChildClassC testClass = new TestChildClassC();
            ((ITypeDict)this.typeDict).Set(testClass);
            Assert.That(this.typeDict.Get<TestChildClassC>(), Is.EqualTo(testClass));
        }

        [Test]
        public void InterfaceSetInvalid()
        {
            Assert.Throws<InvalidCastException>(() => { ((ITypeDict)this.typeDict).Set(3); });
        }

        [Test]
        public void Get()
        {
            Assert.That(this.typeDict.Get<TestChildClassB>(), Is.EqualTo(this.typeDict.Get(typeof(TestChildClassB))));
        }

        [Test]
        public void Contains()
        {
            Assert.That(this.typeDict.Contains<TestChildClassA>(), Is.EqualTo(true));
        }

        [Test]
        public void ContainsNot()
        {
            Assert.That(this.typeDict.Contains<TestBaseClass>(), Is.EqualTo(false));
        }

        [Test]
        public void Remove()
        {
            this.typeDict.Remove<TestChildClassA>();
            Assert.That(this.typeDict.Contains<TestChildClassA>(), Is.EqualTo(false));
        }

        [Test]
        [Ignore("TODO")]
        public void Enumerate()
        {
            throw new NotImplementedException();
        }

        private abstract class TestBaseClass
        {
        }

        private class TestChildClassA : TestBaseClass
        {
            public string Alpha { get; set; }
        }

        private class TestChildClassB : TestBaseClass
        {
            public bool Beta { get; set; }
        }

        private class TestChildClassC : TestBaseClass
        {
        }
    }
}