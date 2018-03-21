﻿IF schema_id('LookUp') IS NULL
    EXECUTE('CREATE SCHEMA [LookUp]')
CREATE TABLE [LookUp].[Source] (
    [Id] [int] NOT NULL IDENTITY,
    [Name] [nvarchar](500) NOT NULL,
    [DisplayOrder] [int] NOT NULL,
    [CreatedById] [int],
    [CreatedDate] [datetime],
    [ModifiedById] [int],
    [ModifiedDate] [datetime],
    [Deleted] [bit] NOT NULL,
    CONSTRAINT [PK_LookUp.Source] PRIMARY KEY ([Id])
)
ALTER TABLE [dbo].[Invoice] ADD [SourceId] [int]
CREATE INDEX [IX_SourceId] ON [dbo].[Invoice]([SourceId])
CREATE INDEX [IX_CreatedById] ON [LookUp].[Source]([CreatedById])
CREATE INDEX [IX_ModifiedById] ON [LookUp].[Source]([ModifiedById])
ALTER TABLE [dbo].[Invoice] ADD CONSTRAINT [FK_dbo.Invoice_LookUp.Source_SourceId] FOREIGN KEY ([SourceId]) REFERENCES [LookUp].[Source] ([Id])
ALTER TABLE [LookUp].[Source] ADD CONSTRAINT [FK_LookUp.Source_dbo.User_CreatedById] FOREIGN KEY ([CreatedById]) REFERENCES [dbo].[User] ([Id])
ALTER TABLE [LookUp].[Source] ADD CONSTRAINT [FK_LookUp.Source_dbo.User_ModifiedById] FOREIGN KEY ([ModifiedById]) REFERENCES [dbo].[User] ([Id])
INSERT [dbo].[__MigrationHistory]([MigrationId], [ContextKey], [Model], [ProductVersion])
VALUES (N'201803190241516_AutomaticMigration', N'Cpi.Application.DatabaseContext.CpiDbContext+MigrationConfiguration',  0x1F8B0800000000000400ED5D5B6FDC38967E5F60FF43A19E76073DAE3841033D813D83749CCC06938BA79D6EEC5B43A9A21D212A55ADA4CAD818EC2FDB87FD49FB17962ADD783D3C9428AA6413FDD0B1481E1E1E9E1B59E4C7FFFB9FFFBDF8CBFD36597C27591EEFD2CBE5F9D9B3E582A4EBDD264EEF2E9787E2F68F3F2DFFF2E77FFD978B379BEDFDE2B7A6DE8BB21E6D99E697CBAF45B17FB95AE5EBAF641BE567DB789DEDF2DD6D71B6DE6D57D166B77AFEECD99F56E7E72B42492C29ADC5E2E297435AC45B72FC83FEF97A97AEC9BE3844C987DD862479FD9D96DC1CA92E3E465B92EFA335B95CBEDEC767AFF6FB245E4705E5E5EC2A2AA22F514E288D82DC17CBC5AB248E285B3724B95D2EA234DD15C78A2F7FCDC94D91EDD2BB9B3DFD10259F1FF684D6BB8D929CD48379D955C78EEBD9F3725CABAE61436A7DC88BDDD692E0F98B5A502BB1792F712F5B415251BEA1222F1ECA511FC549251925C9D576B910BB7AF93AC9CA6A6A595733745635FE612154F9A1D511AA4AE57FB4C621290E19B94CC9A1C8A2E487C5F5E10BADFE37F2F079F78DA497E92149584629ABB48CFB403F5D67BB3DC98A875FC86DCDFEBBCD72B1E2DBADC4866D33A64D35B47769F1E2F972F191761E7D4948AB078C186E8A5D46FE4A52924505D95C474541B2B4A4418E92947A17FAA2C2224D6FE5BF3F538557740813797D9C1B92957F35C4A81653EB5C2E3E44F7EF497A577CBD5CD2395F2EDEC6F764D37CA9A9FF9AC6D49869A3223BA03BBBFEBA4BA1DECE91BD598EF58A2431F5440F6AC1C16D5F6D3619C9F386592DEB2F9C08EA866AC8219794C920DD8C947AF4F343BF767DA4426D35BE8D7BF4D9344475FA31FA1EDF1D4D4637E2E5E217921C6BE45FE37DE59B6B0FF23B53E96DB6DBFEB24B5AD7D495FD7EB33B64EB92959DA6C2E728BB23059EB74E3410736C2D89BBAE50C71E53C396BF4AC520DE9A1A125F55818EA7BA54C5CFC5AA8B1160E4A0A134EB1D39AAC62172E8FB2A25941A1CFE8F0EDCD85B3A04433F23B9FA8FF1FA9BA1E717A3747C1DE5F93F76D906E8F8472721A2E9E8264A8AD13BFB4CB67BC4C89EBBE8AB54CED2A1980DC24CE7D37A7DD81F4DC6323EBD8FF2E2FDEE2E4EFB44453A2551F6D0B622EB781B25CBC57546FF552F837E5A2E6ED6513916FB8151079B154E92BFDFA2CA355E450FF9E7887ABC61120F09081F47AB40A44E40C432299A4A151C2720357D75022215EAD81B9080F0F609F128D694F8E42BE878156AF5E1B7A467E2B4AAA3E4B1FC0071772C1F9438BDDFEDBEFDBAE747DA3B91AA889DA96986FC4ADF9761318D8DC9B6CBDB38DF27D1C3A76C43B2E0C85D3A72B505A81DBBA9AE64FFC6068E1DBFA63F75203056C60EC710287A38B8B23B77AEADA2169C5A706A4FCFA955BA6F7267622DC0F2A5AAA3B9B0BA2793F392AA99991F98D99654AE49B68DF3D27BA8F7D9A43E950D81F128EA9B07A66A3478B38E273868EB4E24157CB2BE2F577B259DBC8752AA6CBEA1F1F38E3217A5D654DE6CE262288D2B9290E19C84A8212F74450BD5EF70E86B2A97C140F511763FA4DEF47B214055DC400644938E0E7E246C1BC348BA3F702361EA3BDD41192518361C5A05C3460C03D6270E4221B74209F130AC519E62B491B51F5AA520230D587994950A22CA8015314370BBC152FE965FFD90EFC681B1F482030B0EEC69393056FB2107A6AEA7B17E4DE5511C18D717E4C03415314370E5C04AA2AF77DB2D25573C0C3A7ECA50093E0BB05D2AAAC1DB068DA88712FAFB21AA990E6ECCCA8DD149D41E06640CE1F7AA227F26502C571E0D942AD97AAA96008ECFAE36C06CF36F33C76D4D6BB68D8754B97E74A7559595CC6C8F797E95ED487F90555DCBC8B9B370303C14843070BAA9EB7516AF3BB7E7FAA8DDBB345A17F1F7B08BECFA68BED1E3E1BC9D174F67F672480F3782777B977EDF5103E8EDDBDAF6C1B3E9FB727AD1CA74F70979867AAC6B5E0E0E70D757A51C1CFA87FBF9B42FE7739759FAB7E616185D78DEDE5AB6ED752FEBFD6EDDE7087AE53E2C1B7DA62A9F8C1B133FEE0A4885E83F5DDC140C01930B42AD9F56874B45B1147E54756C4325673B064685BA2A66B92A00C37C3D5BA66B3A4DF88D89FA344ED79DAA818A7DB91E30064565DB81348EC4C07E574DC574530AB0DA56719C49753DA8F3285539C0E680DFD49BD06160B3ABA662B22905586CAB58DFCEAC4982EC359554CC5565006B750587D74619DAD2CD51B10C62CCC1FD51C1D8FAAFFD6542215106FAAAA4153683E79E8B803BADB24DA8375BA17A3AFBF7B1E5AAEA0A48AA705B1160E59E19CB78698A782E0A95D3B8CD02CCDB2A6045CC04B83D12D164456E0E4474D44240D1F7158E43CC2E74208E4374BA0F1D8650D5D29C2350561DE52004D313740C4259CDCCBC5B875575E7C65D35B482B30ACEEA6939AB46F3215725D7D1D8BAA2E2286EAAED0772528A4A26B6DD3AA83A657379CE542019DC5570574FCB5D090600792D6D558D17D0D71FC58789DD41AE4C5F173916578EEDCDFD9EA479FFA4AB6D1F3C17E0435CFC1E6F707F23E1D2D5F33BDA8FB33D7F6BFE8F28DD243DFC5AF82D780247DFFA08B57357144B4E5055C7D689B73A636092A9A762B22D0698ECEA38FDC9B4EB40F593A95C0A7038D64FA65D0FEAF0A72A07D8EC19E65EE5F96E1D1F79ABD996A0EDF8E1BE49370B0EDD4CA18A95D3E8C0D3A8E7A0F12C2E031E65E172F907498800D57643DB44F5D9D9D9B94498863E9295B1274A5E53F1D3601AA7851C27E3741DEFA3C4C083D00E19614BE1B73D882557844E60191C0D42C574CD79539987B62B21FE9B4474B1629404A53B8C421BA659A5DA0EB447610CFED54766C29FFEC872C5F4CDC7C64934C888C7A69B793C385BA7083A4845BCBEA111DE3CAA1F96270FDA889D9479783733BC9EA59AC0DE6F34ED9CD63BA2999A4E3F67EB3DD5A8BA8660A983D875108735B8BCD64AEE3C36AB19F3179FD532C7F42F639F4FEC0B6BB81D9B082D3681BD5F55BB9FDF937A9AD4E1E9B8F1EAE974C29F5B0CAEC761157DA536A36BDEA9C45A2D3B53E8DEACE3AB049B86DB29D137D3455CB1856DFC057A9C68516CE6C85304364FC63CBCA109EBD04A3DCC7B31CE3572FA7D1A044B53E9E46CBD2408ED06474EBC8754A131DAC6E9D3F18E187EBC45EAC7E01961743E0BB5C0E48A0EB5F014F2C5E9FD216A1E66E20D4D28BE5601B3FBC35B8C66BAD4EEDF401D8E19AC65DEA60AD6F2C46038E95A9DCE825A05CE8C5EEE82CF16E097D8B2CE58F6A9F19DF6C6E166D90D88658AF5372031EC1664D9FC14B6B94B3E301BDCE507E75BDB47A2E0A63656BD87EE65B39CF8DDC566053B0BE591DE8ED64DB4FE21E96EA6AB3A36EAA37D7DDA639AA7E3C183E6E8843A8F5585FCB4B7619AE1D5436FED997695A065C29FFECC763500A246C3298F09425A8C3F3CD0BBEDAAD48041ED7D510AF3E32D8B8227611E5E0C06FEB6500BCCDE88432D3C85BD110343D3E8E16CBD61EDCD6B502143D013E1851CC4510190C84A799DC7539E197FB194972BA6DF0E2B7132AD9170CBA199D68398F33AC4814AD929931602DDA8A60E9548C784275DD289191515EB9706A65128255E9076BA61D42066BE7BAA130834E433CF8718F1A15390A0E7916A69A09030536F583A0ED7AD89179120279EB56BD609941AAD0D1DAC64DCB67182A204F986D3E6B1C2A3C8CE143152143DCAA7B1088AA7A173A89D547CD474A873536FB24E1E414D829F4914D5BF48835505F346AC239D9B7E6B76FAC86A94FD4CA2AB0A9A5DA707204E7BA700CCAB1E781D83F0DD3D2A17C08607AD02043C0F2FA6C3CF37CFBA064CDF9956A941F827D12C252B5EB54B296C0C07D2EB23536A99DDA20087E72CE95BCF90890284F6BA3AC070E44F091FC11A0104E2B6D10B442C75A88527105DA75E3260266156F1D672E18084261F510BA75D41A0189A460F1FC13A4201EC6F4ECFA0C788103920749816F794C030651F9811EA47EF352DD4CB07C546F77CCA84473E94E8EC3AD5C040B58BBF97B30F29D81EF50091DEBD9FF380B8F1A07766E1CF230643D0FA6875C01C3372A479A770C40864670ADD7B0471B745813407421912D2D93E8B042569A1C6636CB688FC780DA8A29C319DB318B8136B147E29815D41F4D5AA93582F4CBC4C781C5EEAD3BE7E94D23CE94DD511F4A9253DA936895C78D52551BA98CE9B3613E75A8A474DE0EC077AE1440C4FDDBB43B63916F03C8AF7044BCF8BB7EC4A2FF439E5F5AAB768906A80C9E91D68DB29E4F30033FEF5ED11444915DFCA40263E94EC2C420A0F2CA315778C38C9F3E2354AF2F2C5745D559DD86BE9DFD1819D0AE2511D5111A467B06C1D99F9611EEFEECCC89237A7669C9039C552E04D243B15C14456E77A790A71D6CCD3649AF918A22E7C474FAC3846D4355CD63328F528C1D7F7953D9D9851C177DA5B7BAAB78D74930E3E74D4CD3BF3241B5EA1A007923CFA2C800D0F7A0408781E4153F5089579C6152F5239D326F925AB49B44962C3AB364902C6F4CE3D5137B136197F2882DE0E73A64BA7F143919E1FAF3A35CB1F8A94AFBB99271E4EDE076AD5B4493AC487577D9A4932FEE6F85E1F6D53D016246B22EF3EBEFA527E24F785E2215A3A89F55BB479FD02A7A81625D11B5270001CF972F1A67D1D50002B90F48A2750698D8A40A34F0602EA07605404754FC5A03B2815C144BAAA831A7659950593D40941AC87E4D8445A85BC8A22CC829AE809F3D02706C2C28D2F9D3E71C7064D240DE42C48B58B26152166518A2363604C754212352D5D44D74F0A1BF551449B9D683DC96EAF1A45505864EBE94AAB7103F9D63FAB4832814F20C3384FD129B16FE832D538F7A45B8B721154ACDA86E6760C8C1F94220340A789C2663A5C94A015F14260D20FBD1474398A8A7D458AD24F0E8A74641441189F9454C8C5EE194AC506A9BEA94A6ADA20084811FDF4A44FA1C2DA66F9802266B8B0363A94AB2F6D553E100798AE50D16875EA97E77A9AB0404C2118E40C0CD2C02A67C31AB4541BA30A622358D9DA4413A566126D4F865BF78B3559B9BAC5E0B066DA4B723E4D534CE6CD4906D002B62F7D439DD52A5624061B06FAF02F4A73AA0235B11CA8398D71204DBF290E5E2FF12F34292C0FAF93CA85ACD1B2A7D047F8B9209C042D3CA385263A90E1A4FE9179E9066BD4BAC771CCF6A6781EC7BD512B1EC4B19B2F17515BF54A0B267C1B5F7781432DF4BE8BBB800EBDE8623B9FC352F092BC21F93E564165CAE5070709F7910C986A6B84DE4314D263190A59C00F6A70A3D03EA9C10CA3DD9206A4A17D446314BF26BFFAA097022A08E81F87E82707BF6986E6F102ADF7C13C76A0F00786E70E24C517B6B58DFEC6F0C0812F0162D20C1454BF6988983463900C7D69210F1AAF3745D5893595F50807D6FA99A07038CD4EB23D852061A06B640163A54BA3D1A2A50B92E17F083188488B8F6E16771FE928718754E2310314F14301B189D8B1A08503E20F8D63406A441C837870910D86CD192A209F2E468DA086313135DC1A6C1212DA9A33639390D59093E0426CA6B4D1DAFC8C40C2CEC4E62FB5B43047240E2E383273D6D95B68BECC5385CBAA109711BE951B0C04E0CA8C823D4C008806826C1D59263CBA282817008854331A3514E910F9A8C147C79411DAB3E3B134556343FB77E5C112B3ECA6F2F220B623528A787B44BB7B1752F46AAA78BF6F8164681A22ECFD1DC8D07F0C5060EA814ECF84C1A7F153000ADF10F707E0EED94D4CEFED08252A9C762FC28C21A7D8270051E4A4B5327712CEB80701E2C68DB88BA30635C3C8CD6207074641732739FF56DB5EB4006D557D1D43634AD2858C2176295DC1B011F620C9A062816508C07A7EBC7CFC6B4C8B8E044A458DA1A4198484A2344422126ED288CE4781F2A3F53C26442085670030812433608E181B3D0E8002E44358183F6DC4B4818685F1D1BDE4E5DFDA1AFE215B535F9FC160B10CB133017D052FE0DE0AA4470CD1AA1112644431E766981169C4F2A17CA36699814546344800EB022D500BF34420648C21D2096C56FB93188CE3A03334ED0F63F6366BF879CC24EE1E6251E10D2824638425E0460401133083622FB300F281A008465115D5AD7950269ACBF59A51C8D7EB87C844BE503FB24CA02589E986B8660CD092C45A20BE9724CA3BCCA06C504E19BCED3C443EAE1DEEC54AB883DB965DAC6ED65FC936AA3F5CAC689535D9178728A14C90246F0A3E44FB7D9CDEE55DCBFACBE2661FADCB9F50FE78B35CDC6F9334BF5C7E2D8AFDCBD52A3F92CECFB6F13ADBE5BBDBE26CBDDBAEA2CD6EF5FCD9B33FADCECF57DB8AC66ACDF971F1C670DB135D93447744282DB78537E46D9CE5C55554445FA29C4AFFF5662B57636F1CF3A26BA5DB74C55F2A96E7ACB9D6D7D42FFFDDDE6B3E7BB52FAF9D1F27E2ACE1A9EEB8B98E2C90ECE4F9960E714BD2E2385A221D2A901BD2A637EB2889B2E6AA3773CDFCF52E396C53FDB5737D6BCA36E1DB575FF0145E1FAF7493ACFC8BA7C497D853BCFEBA4B3524EB228B71D63FDA28C6CB95E029BEDA6C3292E73745466793272914E1697688472C393D0E12204316F0869320848463A4278B8F2BC0D3E3D10958827C893D459945BE4449915A6DB997BD4B17BF45C981B679262909A1A354B5BE5809162D7A9195E446047F2E7A2594CF6AA2834B9F552320D8FB2C5DC3717C56D95B2A799BEE2B9ED2DB4392C894BAAF784A1FE3F5379952F7154FE93ACAF37FEC324142DD577B4A375152A8A95525788A9FC976AFE68F2FB19BCB322D123582FD6E47ADBBB6A9A2C997E229BF8FF2E2FDEE2E4E65F722145978FBF2DB83E0EBEB6F5631232B64AE98CF785ABF45753A1E3DE49FA36F24E5692A8A434432537C2211497773DA6584D240EAD8472C2CA17122989C2BDBE6C85771BE4FA2874FD9866482627025C13ECD149F9C7D36D7ADC6B1CC8AFA209BD49108D618ACF19158A3FA2EA8EBD59CD843CFB59D99CC782B3D77AB836E04223DBEC4D6AA54066543E50D555D9E46F5C56A778B28F6B588E54E5EF0398FDBE7A8AEFABBCF0106FA1B0C919007049B7C5436C9DFB6756F931C386A5F9B8489049B0C36F9486C52BA53E8FA8761166CB7DF2FC42085714CB1EC56D2F8FA9B0595867189145B80A7F7F74374940C4FACFB1ACCDA4CF1A998F568263DCC9CFD9BF2F0A87A9DC56B8144FD094FE35D1AAD8BF8BB40A6FB1A4CD74CF189982E737ED7A5E17658FAF6660BB41DC768DD1DD57279B0CAFDF1AFFAF494F248950D1DF67D739614F4EE39207DF6D6BF48522A9CE2C817FB8410773E01785A08E0AB7DF996E34BFB1EAE9ED2E75D11258A78C17EB7085D3BD10AAA2F215898293EAD60315ABAA77831A577F89820F9AB7B9688749FC36A2E58F9C95BB97C81C4FDB629F37C51DF4D5388C4A92EEEC29669B0C65ED6D8DD64766F8BEDBB5F7D2D514F20D861B0C3476587D2ED64F7E6283E97D7D72A8D74827106E37C24C6C95C10766990DD6393F64608B43DD55DCCE1A65B0F5A3873D77C9C6AC78D7B559E25073E370FC829EC940587C4168A17EDDB2A6DEFF597F6EF1616A0BE92CF61051C6558DEFC3FCA2EAFE101C43BFA5595E5820AFA7BBC29EFE7DF3CE405D91EBDD8D9CD7F25AF93988EB7ABF0214AE35B92179F77DF487AB97CFEECFCF972F12A89A3BC426FA8D1075E8A4F89A3E008CE5F94700464B35D89CDED410D4A2A79BEE15E2760FC38736A42F4AE177F230FE204378A08BD1F7FB1121B5E287C73F50C715CCAF4E8EDFF4AD2F2070FB2B98E8A826469598BD4FB691F0F49127D2951296EA344767E22F94AF9AB0E36F4DF455C3A5E4B22FCEF4215B1F47B94ADBF46345DFA10DDBF27E95DF1B59CFA672CF1E3EBF048DAF5CF5800F1739E386EF81C54805E0C184E058C0080D317F662E87E3E629441D4F897EFD20DB9BF5CFEF3D8E8E5E2DD7FFEDEB4FB61714C5E5F2E9E2DFEDB7A0AD8D860D73DD3D401072E66898F2376A361DB0E190E1F79868DA70D4115992FE53503D008D8C003FA391504C06CFD5C872500B9A71F6DA5DF010B38764C1DCE00E847ACE97637FA01BA3FDAFB271E7FC02D6D1E89000C2FB6A4D96B4776BEA06BA9F30468BDE4110CECD960DB0F714B02ECC130BFD4C01ED454C83ADE46499913D27FE5C7E4EEFC276AE63465A7C5CFADE5C680210C4C5E141008CC14D8664221543EC55089C11E986DE83466F53FF649BCB91DD56070C1E07A1A9C0A5820985A30B5606A0E9781305AC06CCD6DF2ECFF9A4329B063816D3B888906DE00AB3932850ADCA07FFB06D8A03F85E09B9EA46F32210BCCD63385442018DB691A9B1E3220185B30B6606C8E8C0D440498ADA535E002967A7C6C352CC7652FAF58F6DE351DC44277E5253883E00CEC9CC1A37304A384DCFAE6E948BF3875280661991AECD7CA7E35C002B3B55EA787C6CC07BB9EDBBB028B13693D8F79D99CF8C090FDB4EF300CEC6CA96B39C49224C0033B2684E6433899F2A81B7BCEDBAEF7AEE5A0B1B7680C9663AFDB0DE99B456F18298656C7D579C3F9B76D74FFEFE1486208A4F840FAF8F26106AAC16EFEDB8661791CBCC213F50A302CC36C7D42D8960EA6769AA6A6C65D0886160C2D189A534303A11482BD057B0BF6E6C8DE34A809B3B531275BB44643ED73B3AD456518698F69EA2D3C0EDBC18E01A6E9100EC22E1BE487822F147CE1AB3CDFADE3E3D06BD2D5CDDF6E0E059FF826DD2CCAC3CFED15E19A8512CBA179717CF1E19014718970437BBC5C3E3B3B3B970624D2A9B0208CB4FE2011A29E9864A5A38C92D7BB342FB22896F136AEB3385DC7FB28E17917AA21FD7B29CD96A0587245A8872B1DB73C3A4C6F2014CBC5AAA52E041C93102E56CC44A3E6BFD3FAA0003E150086BAF1A201EA0B8D1E3D82E646254B575765E6CA827E10FB143D87466F3C7A92A0383D14E7043C4E1D7378F6B5DAE26C965171C95F4C1A30874E6214AA2719B362625F53DD019E243AD5D78FD58AD7143E1AC7A27D08FEB46351AD1F9344A1A0209A3E4F24E688579B3DAF7AA59BD52255B9C2CC9505F988FD297A14A5BE785E2507859953562B5F50F69EA5689545553C7355413D577F8A9E45A127DEB395A028C65E4FC0A3286350F78761C53C608EE712987A4EED5441EA9A8380398D85923C981CB113D363A5332F9DB25CE04CA5512CAAD1D45B79251FD3A8CE34BB767ED5632E0A5121E67BCC7C6B887E964EF369E6898BEAF1014D4FD367B5F5BC7BCC64C3C49F46962A6305795FF77240457264E18B67AE1C2668264DAFD37B08859E785FF7064531F67A021EA58E2515DB869472C08C9E587CE9395FFE624D77597F32AD606E031FA1A2B4CA819E3C9306B0F78F456A5CD9283A61313B0EF4007CCE5E15536A88AF69D48155057FAB0D9D3A8CAE0A1ED34F5B35983CB56075C1E70A2428C3A9A60F9C7768FEAD8F16F61379E271C372D6A6081E2C0EC569E889D73DAB6915C4EF66C60C238AA01B9EF7B58272E8BB3C8108D382FC79F4191DB0204B8AF93A7355D000276A3A9BDE43743AC0E1E1053DF0AA071294E194BA3069D2A9C04B5328C8A34B3D4D30719A5EA7CF3E553AE33B984CAE2CDEDDCAFC3251859E784C4683A2CC312B15C61013FDAF2896E9C41C74C52E8398465D1810D0097F8CED7014BDFF64CF4038CABFDAB1853377273060A5A6CFE9A38EA41FDE7FAA0F0A02F47952D1A6E1DDF04B7DCF093DC9E570AF49F3BA3066A1B126D60FFFE96AD81C393D2FF1695F3D5D1174C0AB0E34629F3893686084BDE7992D7EB11C73BAA2996B0484D4ACE9F15432CC562FBCE79741314E3F66549C1BF2CA1E13799211A4C764798D24DD7B4513FA0B0125DB7B381151BA659D936A3C0A1F02A2936B3A3E951823EA8CF75013946656D70F98F883B9813078764F381AF59941BF4169DA3B092D94BCC740D4C1D7B3A498AF33F71D1A787E4D67D307994E075A4CF7A0035E758083E19F580726DD0C9F462DA6D80CB75390C937C33BFDF0987C0627316962F9E6F802036D53D016246B0F9E6DC8DB38CB8BABA888BE44B9BCBB51B6BA2105771776B978D3BEE7205C67BC597F25DBE872B9F9B2A3F35CBD0871BC1A292B064FB9D129897253A0A25C961929EBE098A59E74159B9EAB72AE7306301ACF46A92F06069A2A40D765159454CB8A3C529152C672359DC4F99AC8811B385055D20CDEBA6BFED6B5A66BBE92A6EBAE92B16BE96E84D264B81A3ADBE90EEF1A3B853B347686EE8859EF49DD3065AA4EEA626C17F0905495804EF10394531F8DDEB055345AD32661B84EBB6D624D975D054D87F54635AE3B693B40D3AB544FD33957CFC80313FFA57E9932D59CB6CF79095D3001500C2FCCBA78C154E3228D6EEDAC4C8ADAE1749FA4582EB6AADE9A31B6E42239AD871F2493D4E947A9CBFC66304CE313318A51DB3D2BE3400870D671A4A2AB32968060BDB07C4065E622629002D91C4E6B2E42450F83EA27E041DA52659D5843926A8FA71F7CC62C08B129742F08ACC1C8D5E72E0AD582C01C3C811663C418DDC2A6A52157184730E6800B35792CA20111E7B5F6E3495FF48B50C684C61708C6A3F8D2932944A2B40576418F341F1DECB8DB019E9059316146EE2747852755BB9122CE49088EC9DCAABD3230B93B56F1258EB1523909EE5831661812D9815FE1F7888FAD9A4FAE0608BB5103F8EF890F1104B0D5DA3906F0D659CC506DA83286C0178F21104C184541BBCE54243C24A9DE04542786DC32EFD5F225BC4DCDC8615CCE7E2C4BAD141BD36D73AE6CF8D095A00CAAB19BD11B5C383FDDC0471BB4C1DD23B008E6336C2D5A2046D7D5282F438731A5EA6B41F130E21835C5994E08E6ECC797497815830AE64C317E231A9A83814B3F7F1E1B325F1D0E96C7F302070C407FCD69D068FF8707BA72EA02F5BF4CB3121945F9D1BE108FE7E44E33A61608EC172D908BE62D1235E20EE8384C083D43FCC0E46201C065B48B4733148DB395A37CC684597AB185EE0581594623405766280A055C086820EA73D4EE8630597681F2995E5CA597E1B6B00FE060D5E010B318AA16DF406BE0262C0467D62D9E6B630CA32B722D008C8B33DEFA9F9908A41BEBA0B2AB6EB5BB617D12D5D75FC9D6CE3FF216B7332DD09CB864242AD518493818EBC0DE579EB578A44BB6B0C9E0B6D5070FC4B301A9AE8F2AE460BC65EA4013A463C1C786CC578783EDEE498283D55CA79CD960A174D7746D70A474D7E7F0618767BE177752B37DB1122E75B56517ABEAC07CFD81FE49B3D9E88ED06191243F7EBD58FD72A0ADB7A4FAEB8AE4F15D47E282D24CC9BAECB323DAD47997DEEE9A1B6D02474D95A6B8B9ED468A681315D1ABAC886FA375418BD724CFE3F46EB9F82D4A0EA558B65FC8E65DFAE950EC0F051D32D97E49B86B1AE59D38A8FF8B95C4F3C5A77DF957EE620894CD980E817C4A7F3EC4C9A6E5FB6D948877107424CACB767F2569B9B628E7B2A0FF27770F2DA58FBB1449A8165F7B47F033D9EE134A2CFF94DE44DF491FDEA8E2BE2777D1FA817EFF1E6FCA6B753A22E689E0C57E7115477759B4CD6B1A5D7BFA27D5E1CDF6FECFFF0FC431859752250200 , N'6.1.3-40302')

SET IDENTITY_INSERT [LookUp].[Source] ON
INSERT INTO [LookUp].[Source] (Id, Name, DisplayOrder, Deleted) VALUES (1, 'Hang Meas TV', 1, 0)
INSERT INTO [LookUp].[Source] (Id, Name, DisplayOrder, Deleted) VALUES (2, 'Facebook', 2, 0)
SET IDENTITY_INSERT [LookUp].[Source] OFF

GO

DELETE FROM [LookUp].[Location] WHERE Name = 'Prey Veng'; 
Update [LookUp].[Location] SEt DisplayOrder = 99 WHERE Id = 2;

SET IDENTITY_INSERT [LookUp].[Location] ON
INSERT INTO [LookUp].[Location] (Id, Name, DisplayOrder, Deleted) VALUES (3, 'Siem Reap', 2, 0)
INSERT INTO [LookUp].[Location] (Id, Name, DisplayOrder, Deleted) VALUES (4, 'Battambang', 3, 0)
INSERT INTO [LookUp].[Location] (Id, Name, DisplayOrder, Deleted) VALUES (5, 'Prey Veng', 4, 0)
SET IDENTITY_INSERT [LookUp].[Location] OFF

GO


SET IDENTITY_INSERT [LookUp].[Location] ON
INSERT INTO [LookUp].[Location] (Id, Name, DisplayOrder, Deleted) VALUES (6, 'Takéo', 5, 0)
SET IDENTITY_INSERT [LookUp].[Location] OFF