using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using TestChallengeProjects;
using TestChallengeProjects.HouseKeeperHelperProject;
using Xunit;

namespace TestChallengeProjectsTests
{
    public class HouseKeeperHelperTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IStatementGenerator> _mockStatementGenerator;
        private readonly Mock<IEmailSender> _mockEmailSender;
        private readonly Mock<IXtraMessageBox> _mockXtraMessageBox;
        private readonly HouseKeeperHelper _houseKeeperHelper;
        private readonly Housekeeper _houseKeeper;
        private readonly List<Housekeeper> _houseKeeperList;
        private readonly DateTime _statementDate;
        private readonly string _statementFilename;

        #region Constructor
        public HouseKeeperHelperTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockStatementGenerator = new Mock<IStatementGenerator>();
            _mockEmailSender = new Mock<IEmailSender>();
            _mockXtraMessageBox = new Mock<IXtraMessageBox>();

            _houseKeeperHelper = new HouseKeeperHelper
                (
                _mockUnitOfWork.Object,
                _mockStatementGenerator.Object,
                _mockEmailSender.Object,
                _mockXtraMessageBox.Object
                );

            _houseKeeper = new Housekeeper()
            {
                Email = "username@domain.com",
                FullName = "fullname",
                Oid = 1,
                StatementEmailBody = "statementEmailBody"
            };

            _houseKeeperList = new List<Housekeeper>
            {
                _houseKeeper
            };

            _statementDate = new(2022, 2, 15);
            _statementFilename = "statementFilename";

            _mockUnitOfWork.Setup(uow => uow.Query<Housekeeper>())
                           .Returns(_houseKeeperList.AsQueryable());

            _mockStatementGenerator.Setup(st => st.SaveStatement(_houseKeeper.Oid, _houseKeeper.FullName, _statementDate))
                                   .Returns(_statementFilename);
        }
        #endregion

        #region SendStatementEmails_ReturnFalse
        [Fact]
        public void SendStatementEmails_ReturnFalse_WhenHouseKeeperEmailIsNull()
        {
            //Arrange
            _houseKeeper.Email = null;

            //Act
            var result = _houseKeeperHelper.SendStatementEmails(_statementDate);

            //Assert
            _mockStatementGenerator.Verify(sg => sg.SaveStatement(
                                        _houseKeeper.Oid,
                                        _houseKeeper.FullName,
                                        _statementDate),
                                    Times.Never);
            Assert.False(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void SendStatementEmails_ReturnFalse_WhenStatementFileIsNullEmptyOrWhiteSpace(string saveStatementReturn)
        {
            //Arrange
            _mockStatementGenerator.Setup(st => st.SaveStatement(_houseKeeper.Oid, _houseKeeper.FullName, _statementDate))
                                   .Returns(saveStatementReturn);

            //Act
            var result = _houseKeeperHelper.SendStatementEmails(_statementDate);

            //Assert
            VerifySaveStatementOnce();
            _mockEmailSender.Verify(es => es.EmailFile(
                                        _houseKeeper.Email,
                                        _houseKeeper.StatementEmailBody,
                                        _statementFilename,
                                        It.IsAny<string>()),
                                    Times.Never);
            Assert.False(result);
        }

        [Fact]
        public void SendStatementEmails_ReturnFalse_WhenEmailSenderFails()
        {
            //Arrange
            _mockEmailSender.Setup(es => es.EmailFile(
                                _houseKeeper.Email,
                                _houseKeeper.StatementEmailBody,
                                _statementFilename,
                                 It.IsAny<string>()))
                            .Throws<Exception>();

            //Act
            var result = _houseKeeperHelper.SendStatementEmails(_statementDate);

            //Assert
            VerifySaveStatementOnce();
            VerifyEmailSentOnce();
            _mockXtraMessageBox.Verify(xmb => xmb.Show(It.IsAny<string>(), It.IsAny<string>(), MessageBoxButtons.OK), Times.Once);
            Assert.False(result);
        }
        #endregion

        #region SendStatementEmails_ReturnTrue
        [Fact]
        public void SendStatementEmails_ReturnTrue_WhenEmailSent()
        {
            //Arrange
            _mockEmailSender.Setup(es => es.EmailFile(
                                _houseKeeper.Email,
                                _houseKeeper.StatementEmailBody,
                                _statementFilename,
                                It.IsAny<string>()));

            //Act
            var result = _houseKeeperHelper.SendStatementEmails(_statementDate);

            //Assert
            VerifySaveStatementOnce();
            VerifyEmailSentOnce();
            _mockXtraMessageBox.Verify(xmb => xmb.Show(It.IsAny<string>(), It.IsAny<string>(), MessageBoxButtons.OK), Times.Never);
            Assert.True(result);
        }
        #endregion

        private void VerifySaveStatementOnce()
        {
            _mockStatementGenerator.Verify(sg => sg.SaveStatement(
                            _houseKeeper.Oid,
                            _houseKeeper.FullName,
                            _statementDate),
                        Times.Once);
        }

        private void VerifyEmailSentOnce()
        {
            _mockEmailSender.Verify(es => es.EmailFile(
                    _houseKeeper.Email,
                    _houseKeeper.StatementEmailBody,
                    _statementFilename,
                    It.IsAny<string>()),
                Times.Once);
        }
    }
}
