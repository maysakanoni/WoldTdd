using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Tests
{
    public class Tests
    {
        [Test]
        public void WillNotLaunchMissileWithExpiredLaunchCodeStub()
        {
            var dummyMissile = new DummyMissile();
            var expiredLaunchCodeStub = new ExpiredLaunchCodeStub();
            var launchMissile = new LaunchMissile(dummyMissile, expiredLaunchCodeStub, new DummyUsedLaunchCodes());
            launchMissile.Execute();
            dummyMissile.Should().NotBeEquivalentTo(new NotImplementedException());
        }

        [Test]
        public void WillNotLaunchMissileSpyWithExpiredLaunchCode()
        {
            var missileSpy = new MissileSpy();
            var expiredLaunchCodeStub = new ExpiredLaunchCodeStub();
            var launchMissile = new LaunchMissile(missileSpy, expiredLaunchCodeStub, new DummyUsedLaunchCodes());
            launchMissile.Execute();
            missileSpy.LaunchHasBeenCalled().Should().BeFalse();
        }

        [Test]
        public void WillNotLaunchMissileMockWithExpiredLaunchCode()
        {
            var missileMock = new MissileMock();
            var expiredLauchCodeStub = new ExpiredLaunchCodeStub();
            var launchMissile = new LaunchMissile(missileMock, expiredLauchCodeStub, new DummyUsedLaunchCodes());
            launchMissile.Execute();
            missileMock.VerifyCodeRedAbort();
        }

        [Test]
        public void WillNotLaunchMissileMockWithUsedLaunchCode()
        {
            var missileMock = new MissileMock();
            var validLaunchCode = new ValidLaunchCode();
            var fakeLaunchCodes = new FakeLaunchCodes();
            fakeLaunchCodes.add(validLaunchCode);
            var launchMissile = new LaunchMissile(missileMock, validLaunchCode, fakeLaunchCodes);
            launchMissile.Execute();
            missileMock.VerifyCodeRedAbort();
        }
    }

    public class DummyUsedLaunchCodes : IFakeLaunchCodes
    {
        public void add(ILaunchCode launchCode)
        {
            throw new NotImplementedException();
        }

        public bool contains(ILaunchCode launchCode)
        {
            throw new NotImplementedException();
        }
    }

    public class FakeLaunchCodes : IFakeLaunchCodes
    {
        public List<ILaunchCode> _LaunchCodes = new List<ILaunchCode>();

        public void add(ILaunchCode launchCode)
        {
            _LaunchCodes.Add(launchCode);
        }

        public bool contains(ILaunchCode launchCode)
        {
            return _LaunchCodes.Contains(launchCode);
        }

    }
    

    public interface IFakeLaunchCodes
    {
        void add(ILaunchCode launchCode);
        bool contains(ILaunchCode launchCode);
    }

    public class ValidLaunchCode : ILaunchCode
    {
        public bool isExpired()
        {
            return false;
        }

        public bool isUnsigned()
        {
            return false;
        }
    }

    public class MissileMock : IMissile
    {
        public bool _launchHasBeenCalled = false;
        public bool _disableHasBeenCalled = false;
        public void Launch()
        {
            _launchHasBeenCalled = true;
        }

        public void Disable()
        {
            _disableHasBeenCalled = true;
        }

        public bool LaunchHasBeenCalled()
        {
            return _launchHasBeenCalled;
        }

        public bool DisableHasBeenCalled()
        {
            return _disableHasBeenCalled;
        }

        public void VerifyCodeRedAbort()
        {
            LaunchHasBeenCalled().Should().BeFalse();
            DisableHasBeenCalled().Should().BeTrue();
        }
    }

    public class MissileSpy : IMissile
    {
        public bool _launchHasBeenCalled = false;
        public bool _disableHasBeenCalled = false;

        public void Launch()
        {
            _launchHasBeenCalled = true;
        }

        public void Disable()
        {
            _disableHasBeenCalled = true;
        }

        public bool LaunchHasBeenCalled()
        {
            return _launchHasBeenCalled;
        }
        public bool DisableHasBeenCalled()
        {
            return _disableHasBeenCalled;
        }
    }

    public class LaunchMissile
    {
        public IMissile _missile;
        public ILaunchCode _launchCode;
        public IFakeLaunchCodes _fakeLaunchCodes;
        public LaunchMissile(IMissile missile, ILaunchCode launchCode, IFakeLaunchCodes fakeLaunchCodes)
        {
            _missile = missile;
            _launchCode = launchCode;
            _fakeLaunchCodes = fakeLaunchCodes;
        }

        public void Execute()
        {
            if (_launchCode.isExpired() == false && _fakeLaunchCodes.contains(_launchCode) == false)
            {
                _missile.Launch();
            }
            else if(_launchCode.isExpired()|| _fakeLaunchCodes.contains(_launchCode))
            {
                _missile.Disable();
            }
        }
    }

    public class ExpiredLaunchCodeStub : ILaunchCode
    {
        public bool isExpired()
        {
            return true;
        }

        public bool isUnsigned()
        {
            return false;
        }
    }

    public interface ILaunchCode
    {
        bool isExpired();
        bool isUnsigned();
    }

    public class DummyMissile : IMissile
    {
        public bool _disableWasCalled = false;
        public void Launch()
        {
            throw new NotImplementedException();
        }

        public void Disable()
        {
            _disableWasCalled=true;
        }
    }

    public interface IMissile
    {
        void Launch();
        void Disable();
    }
}