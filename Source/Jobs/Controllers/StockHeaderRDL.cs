﻿using Data.Models;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using Model.Models;
using System.IO;
using System.Collections.Generic;
using ImageResizer;
using System;
using Core.Common;

namespace Jobs.Controllers
{
    public class StockHeaderRDL
    {

        public string Create_Std_StockIssue_Print()
        {
            string StringCode = "";
            StringCode = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Report xmlns=""http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition"" xmlns:rd=""http://schemas.microsoft.com/SQLServer/reporting/reportdesigner"">
  <Body>
    <ReportItems>
      <Tablix Name=""Tablix1"">
        <TablixBody>
          <TablixColumns>
            <TablixColumn>
              <Width>1.04563in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.1616in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>3.55349in</Width>
            </TablixColumn>
          </TablixColumns>
          <TablixRows>
            <TablixRow>
              <Height>0.22528in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox27"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!PartyCaption.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox26</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                          <Width>1pt</Width>
                        </TopBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""BuyerName"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Format>0.0000;(0.0000);'-'</Format>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>BuyerName</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                          <Width>1pt</Width>
                        </TopBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Jobworker"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!PartyName.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Jobworker</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                          <Width>1pt</Width>
                        </TopBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox3"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox3</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                          <Width>1pt</Width>
                        </TopBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox117"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox117</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                          <Width>1pt</Width>
                        </TopBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox134"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!PartySuffix.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox134</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                          <Width>1pt</Width>
                        </TopBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.2357in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox196"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Address</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox177</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox76"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox60</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Address"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!PartyAddress.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Address</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.22528in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox252"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Mobile No.</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox252</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox81"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox60</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""MobileNo"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!PartyMobileNo.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>MobileNo</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.23958in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox149"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>GSTIN</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox146</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox143"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox143</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox150"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!Party_GST_NO.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox145</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.23362in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox115"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value EvaluationMode=""Constant"">Aadhar No</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox115</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox80"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox60</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""TinNo"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!Party_AADHAR_NO.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>TinNo</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.21875in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox147"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value EvaluationMode=""Constant"">PAN No</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox147</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox148"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox148</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""AADHAR"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!Party_PAN_NO.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>AADHAR</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
          </TablixRows>
        </TablixBody>
        <TablixColumnHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember />
            <TablixMember />
          </TablixMembers>
        </TablixColumnHierarchy>
        <TablixRowHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember>
              <Visibility>
                <Hidden>=iif(fields!PartySuffix.Value &lt;&gt; """",false,true)</Hidden>
              </Visibility>
            </TablixMember>
            <TablixMember />
            <TablixMember>
              <Visibility>
                <Hidden>=iif(fields!PartyMobileNo.Value &lt;&gt; """",false,true)</Hidden>
              </Visibility>
            </TablixMember>
            <TablixMember>
              <Visibility>
                <Hidden>=iif(max(fields!Party_GST_NO.Value) &lt;&gt; """",False,True)</Hidden>
              </Visibility>
            </TablixMember>
            <TablixMember>
              <Visibility>
                <Hidden>=iif(max(fields!Party_AADHAR_NO.Value) &lt;&gt; """",False,True)</Hidden>
              </Visibility>
            </TablixMember>
            <TablixMember>
              <Visibility>
                <Hidden>=iif(fields!Party_PAN_NO.Value &lt;&gt; """" ,false,true)</Hidden>
              </Visibility>
            </TablixMember>
          </TablixMembers>
        </TablixRowHierarchy>
        <DataSetName>DsMain</DataSetName>
        <Top>0.67223in</Top>
        <Left>0.03122in</Left>
        <Height>1.62821in</Height>
        <Width>4.76072in</Width>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
        </Style>
      </Tablix>
      <Tablix Name=""Tablix5"">
        <TablixBody>
          <TablixColumns>
            <TablixColumn>
              <Width>1.21556in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.13544in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>1.86232in</Width>
            </TablixColumn>
          </TablixColumns>
          <TablixRows>
            <TablixRow>
              <Height>0.19132in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox39"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=fields!DocIdCaption.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox34</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox52"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox52</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""DocNo"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!DocNo.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>DocNo</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.21216in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox36"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=fields!DocIdCaptionDate.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox31</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox49"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox49</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""DocDate"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!DocDate.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <Format>dd/MMM/yy</Format>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>DocDate</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.23958in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox11"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>From Godown</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox11</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox14"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox14</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox15"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=fields!FromGodownName.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox15</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.21875in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox16"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=iif(max(fields!FromGodownName.Value) &lt;&gt; """",""To Godown"",""Godown"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox16</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox17"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox17</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox20"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!GodownName.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox20</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.20833in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox5"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Process</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox5</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox9"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox9</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox10"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=fields!ProcessName.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox10</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.19792in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox194"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Reverse Charge</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox194</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox195"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox195</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                          <Style>None</Style>
                          <Width>1pt</Width>
                        </TopBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""ReverseCharge"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!ReverseCharge.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>ReverseCharge</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
          </TablixRows>
        </TablixBody>
        <TablixColumnHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember />
            <TablixMember />
          </TablixMembers>
        </TablixColumnHierarchy>
        <TablixRowHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember />
            <TablixMember>
              <Visibility>
                <Hidden>=iif(max(fields!FromGodownName.Value) &lt;&gt; """",False,True)</Hidden>
              </Visibility>
            </TablixMember>
            <TablixMember>
              <Visibility>
                <Hidden>=iif(max(fields!GodownName.Value) &lt;&gt; """",False,True)</Hidden>
              </Visibility>
            </TablixMember>
            <TablixMember>
              <Visibility>
                <Hidden>=iif(fields!ProcessName.Value &lt;&gt; """",false,true)</Hidden>
              </Visibility>
            </TablixMember>
            <TablixMember>
              <Visibility>
                <Hidden>=iif(max(fields!ReverseCharge.Value) &lt;&gt; """",False,True)</Hidden>
              </Visibility>
            </TablixMember>
          </TablixMembers>
        </TablixRowHierarchy>
        <DataSetName>DsMain</DataSetName>
        <Top>0.67848in</Top>
        <Left>4.83321in</Left>
        <Height>1.26806in</Height>
        <Width>3.21332in</Width>
        <ZIndex>1</ZIndex>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
        </Style>
      </Tablix>
      <Textbox Name=""ReportTitle"">
        <CanGrow>true</CanGrow>
        <KeepTogether>true</KeepTogether>
        <Paragraphs>
          <Paragraph>
            <TextRuns>
              <TextRun>
                <Value>=First(Fields!ReportTitle.Value, ""DsMain"")</Value>
                <Style>
                  <FontFamily>Tahoma</FontFamily>
                  <FontSize>14pt</FontSize>
                  <FontWeight>Bold</FontWeight>
                </Style>
              </TextRun>
            </TextRuns>
            <Style>
              <TextAlign>Center</TextAlign>
            </Style>
          </Paragraph>
        </Paragraphs>
        <rd:DefaultName>ReportTitle</rd:DefaultName>
        <Top>0.35734in</Top>
        <Left>0.01872in</Left>
        <Height>0.27183in</Height>
        <Width>7.98851in</Width>
        <ZIndex>2</ZIndex>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
          <TopBorder>
            <Color>Black</Color>
            <Style>Solid</Style>
            <Width>1pt</Width>
          </TopBorder>
          <BottomBorder>
            <Color>Black</Color>
            <Style>Solid</Style>
            <Width>1pt</Width>
          </BottomBorder>
          <PaddingLeft>2pt</PaddingLeft>
          <PaddingRight>2pt</PaddingRight>
          <PaddingTop>2pt</PaddingTop>
          <PaddingBottom>2pt</PaddingBottom>
        </Style>
      </Textbox>
      <Subreport Name=""CompanyDetail"">
        <ReportName>CompanyDetail</ReportName>
        <Parameters>
          <Parameter Name=""SiteId"">
            <Value>=First(Fields!SiteId.Value, ""DsMain"")</Value>
          </Parameter>
          <Parameter Name=""DivisionId"">
            <Value>=First(Fields!DivisionId.Value, ""DsMain"")</Value>
          </Parameter>
          <Parameter Name=""DatabaseConnectionString"">
            <Value>=Parameters!DatabaseConnectionString.Value</Value>
          </Parameter>
          <Parameter Name=""PrintedBy"">
            <Value>=Parameters!PrintedBy.Value</Value>
          </Parameter>
        </Parameters>
        <Top>0.03194in</Top>
        <Left>0.02685in</Left>
        <Height>0.12748in</Height>
        <Width>7.98038in</Width>
        <ZIndex>3</ZIndex>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
        </Style>
      </Subreport>
      <Subreport Name=""ProvisionalCompanyDetail"">
        <ReportName>ProvisionalCompanyDetail</ReportName>
        <Parameters>
          <Parameter Name=""SiteId"">
            <Value>=First(Fields!SiteId.Value, ""DsMain"")</Value>
          </Parameter>
          <Parameter Name=""DivisionId"">
            <Value>=First(Fields!DivisionId.Value, ""DsMain"")</Value>
          </Parameter>
          <Parameter Name=""DatabaseConnectionString"">
            <Value>=Parameters!DatabaseConnectionString.Value</Value>
          </Parameter>
          <Parameter Name=""PrintedBy"">
            <Value>=Parameters!PrintedBy.Value</Value>
          </Parameter>
        </Parameters>
        <Top>0.1875in</Top>
        <Left>0.05049in</Left>
        <Height>0.11428in</Height>
        <Width>7.94632in</Width>
        <ZIndex>4</ZIndex>
        <Visibility>
          <Hidden>true</Hidden>
        </Visibility>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
        </Style>
      </Subreport>
      <Tablix Name=""Tablix13"">
        <TablixBody>
          <TablixColumns>
            <TablixColumn>
              <Width>2.80221in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>2.84586in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>2.36726in</Width>
            </TablixColumn>
          </TablixColumns>
          <TablixRows>
            <TablixRow>
              <Height>0.22917in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox12"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=fields!SignatoryleftCaption.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox6</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox13"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=fields!SignatoryMiddleCaption.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox7</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox59"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=fields!SignatoryRightCaption.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox22</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
          </TablixRows>
        </TablixBody>
        <TablixColumnHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember />
            <TablixMember />
          </TablixMembers>
        </TablixColumnHierarchy>
        <TablixRowHierarchy>
          <TablixMembers>
            <TablixMember />
          </TablixMembers>
        </TablixRowHierarchy>
        <DataSetName>DsMain</DataSetName>
        <Top>4.84666in</Top>
        <Left>0.0312in</Left>
        <Height>0.22917in</Height>
        <Width>8.01533in</Width>
        <ZIndex>5</ZIndex>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
        </Style>
      </Tablix>
      <Tablix Name=""Tablix14"">
        <TablixBody>
          <TablixColumns>
            <TablixColumn>
              <Width>4.60809in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>3.4039in</Width>
            </TablixColumn>
          </TablixColumns>
          <TablixRows>
            <TablixRow>
              <Height>0.17917in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox1"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox1</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox2"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox2</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.23125in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox46"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Remark : </Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                            <TextRun>
                              <Value>=Fields!Remark.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox40</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.16875in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox67"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox67</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.23125in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox226"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox226</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox95"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>FOR </Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                            <TextRun>
                              <Value>=fields!CompanyName.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox35</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.1375in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox232"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox232</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox233"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox233</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
          </TablixRows>
        </TablixBody>
        <TablixColumnHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember />
          </TablixMembers>
        </TablixColumnHierarchy>
        <TablixRowHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember>
              <Visibility>
                <Hidden>=iif(fields!Remark.Value &lt;&gt; """",false,true)</Hidden>
              </Visibility>
            </TablixMember>
            <TablixMember />
            <TablixMember />
            <TablixMember />
          </TablixMembers>
        </TablixRowHierarchy>
        <DataSetName>DsMain</DataSetName>
        <Top>3.8893in</Top>
        <Left>0.03454in</Left>
        <Height>0.94792in</Height>
        <Width>8.01199in</Width>
        <ZIndex>6</ZIndex>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
        </Style>
      </Tablix>
      <Tablix Name=""Tablix15"">
        <TablixBody>
          <TablixColumns>
            <TablixColumn>
              <Width>0.47804in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>2.57381in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>1.13542in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.97727in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.83056in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>2.00422in</Width>
            </TablixColumn>
          </TablixColumns>
          <TablixRows>
            <TablixRow>
              <Height>0.25096in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox325"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Sr.</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox320</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox257"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Max(Fields!ProductCaption.Value)</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox251</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox68"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=max(fields!SalesTaxProductCodeCaption.Value)</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox68</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox21"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Qty</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox10</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox364"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Unit</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox362</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Color>Black</Color>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox261"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Remarks</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox259</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox326"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=RowNumber(Nothing)</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox321</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""ProductName3"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Code.PrintProduct(Fields!ProductName.Value,Fields!ProductGroupCaption.Value,fields!ProductGroupName.Value,Fields!SpecificationCaption.Value,fields!Specification.Value,fields!Dimension1Caption.Value,fields!Dimension1Name.Value,fields!Dimension2Caption.Value,fields!Dimension2Name.Value,fields!Dimension3Caption.Value,fields!Dimension3Name.Value,fields!Dimension4Caption.Value,fields!Dimension4Name.Value,Fields!ProductUidCaption.Value,fields!ProductUidName.Value)</Value>
                              <MarkupType>HTML</MarkupType>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>ProductName</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox69"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=fields!SalesTaxProductCodes.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox69</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Qty3"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Interaction.iif(Fields!Qty.Value &lt;&gt; 0, Microsoft.VisualBasic.Strings.formatnumber(Fields!Qty.Value, Fields!DecimalPlaces.Value), ""-"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Qty</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""UnitName3"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!UnitName.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>UnitName</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Remark4"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=fields!LineRemark.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Remark</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox48"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>TOTAL</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox42</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox70"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox70</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox58"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Interaction.iif(Sum(Fields!Qty.Value) &lt;&gt; 0, Microsoft.VisualBasic.Strings.formatnumber(Sum(Fields!Qty.Value), Max(Fields!DecimalPlaces.Value)), ""-"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox48</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox33"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox29</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox60"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox54</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
          </TablixRows>
        </TablixBody>
        <TablixColumnHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
          </TablixMembers>
        </TablixColumnHierarchy>
        <TablixRowHierarchy>
          <TablixMembers>
            <TablixMember>
              <KeepWithGroup>After</KeepWithGroup>
              <RepeatOnNewPage>true</RepeatOnNewPage>
            </TablixMember>
            <TablixMember>
              <Group Name=""Details3"" />
              <SortExpressions>
                <SortExpression>
                  <Value>=Fields!StockHeaderId.Value</Value>
                </SortExpression>
              </SortExpressions>
              <TablixMembers>
                <TablixMember />
              </TablixMembers>
            </TablixMember>
            <TablixMember>
              <KeepWithGroup>Before</KeepWithGroup>
            </TablixMember>
          </TablixMembers>
        </TablixRowHierarchy>
        <DataSetName>DsMain</DataSetName>
        <Top>2.33115in</Top>
        <Left>0.04722in</Left>
        <Height>0.75096in</Height>
        <Width>7.99932in</Width>
        <ZIndex>7</ZIndex>
        <Visibility>
          <Hidden>=Microsoft.VisualBasic.Interaction.IIF(Max(Fields!CostCenterCnt.Value) &gt; 0, True, False)</Hidden>
        </Visibility>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
        </Style>
      </Tablix>
      <Tablix Name=""Tablix16"">
        <TablixBody>
          <TablixColumns>
            <TablixColumn>
              <Width>0.45716in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>2.49888in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>1in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>1.4565in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.72723in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.52843in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>1.33947in</Width>
            </TablixColumn>
          </TablixColumns>
          <TablixRows>
            <TablixRow>
              <Height>0.25096in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox327"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Sr.</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox320</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox258"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Max(Fields!ProductCaption.Value)</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox251</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox71"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=max(fields!SalesTaxProductCodeCaption.Value)</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox71</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox50"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Max(Fields!CostCenterCaption.Value)</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox46</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox23"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Qty</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox10</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox365"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Unit</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox362</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Color>Black</Color>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox262"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Remarks</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox259</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox328"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=RowNumber(Nothing)</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox321</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""ProductName4"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Code.PrintProduct(Fields!ProductName.Value,Fields!ProductGroupCaption.Value,fields!ProductGroupName.Value,Fields!SpecificationCaption.Value,fields!Specification.Value,fields!Dimension1Caption.Value,fields!Dimension1Name.Value,fields!Dimension2Caption.Value,fields!Dimension2Name.Value,fields!Dimension3Caption.Value,fields!Dimension3Name.Value,fields!Dimension4Caption.Value,fields!Dimension4Name.Value,Fields!ProductUidCaption.Value,fields!ProductUidName.Value)</Value>
                              <MarkupType>HTML</MarkupType>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>ProductName</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox72"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=fields!SalesTaxProductCodes.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox72</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""CostCenterName"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!CostCenterName.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>CostCenterName</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Qty4"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Interaction.iif(Fields!Qty.Value &lt;&gt; 0, Microsoft.VisualBasic.Strings.formatnumber(Fields!Qty.Value, Fields!DecimalPlaces.Value), ""-"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Qty</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""UnitName4"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!UnitName.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>UnitName</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Remark5"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=fields!LineRemark.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Remark</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox51"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>TOTAL</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox42</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox73"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox73</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox53"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox50</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox61"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Interaction.iif(Sum(Fields!Qty.Value) &lt;&gt; 0, Microsoft.VisualBasic.Strings.formatnumber(Sum(Fields!Qty.Value), Max(Fields!DecimalPlaces.Value)), ""-"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox48</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox34"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox29</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox62"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox54</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
          </TablixRows>
        </TablixBody>
        <TablixColumnHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
          </TablixMembers>
        </TablixColumnHierarchy>
        <TablixRowHierarchy>
          <TablixMembers>
            <TablixMember>
              <KeepWithGroup>After</KeepWithGroup>
              <RepeatOnNewPage>true</RepeatOnNewPage>
            </TablixMember>
            <TablixMember>
              <Group Name=""Details4"" />
              <SortExpressions>
                <SortExpression>
                  <Value>=Fields!StockHeaderId.Value</Value>
                </SortExpression>
              </SortExpressions>
              <TablixMembers>
                <TablixMember />
              </TablixMembers>
            </TablixMember>
            <TablixMember>
              <KeepWithGroup>Before</KeepWithGroup>
            </TablixMember>
          </TablixMembers>
        </TablixRowHierarchy>
        <DataSetName>DsMain</DataSetName>
        <Top>3.11108in</Top>
        <Left>0.03887in</Left>
        <Height>0.75096in</Height>
        <Width>8.00767in</Width>
        <ZIndex>8</ZIndex>
        <Visibility>
          <Hidden>=Microsoft.VisualBasic.Interaction.IIF(Max(Fields!CostCenterCnt.Value) &gt; 0, False, True)</Hidden>
        </Visibility>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
        </Style>
      </Tablix>
      <Tablix Name=""Tablix3"">
        <TablixBody>
          <TablixColumns>
            <TablixColumn>
              <Width>0.61043in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.48759in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>2.79631in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>1.26675in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.69667in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.99811in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>1in</Width>
            </TablixColumn>
          </TablixColumns>
          <TablixRows>
            <TablixRow>
              <Height>0.42653in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox386"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>14pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox386</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Dotted</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <VerticalAlign>Middle</VerticalAlign>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>7</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell />
                <TablixCell />
                <TablixCell />
                <TablixCell />
                <TablixCell />
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.32292in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""ReportTitle2"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!ReportTitle.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>14pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>ReportTitle</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <VerticalAlign>Middle</VerticalAlign>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>7</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell />
                <TablixCell />
                <TablixCell />
                <TablixCell />
                <TablixCell />
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox22"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Name</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox9</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox24"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox10</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""DocNo2"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!PersonName.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>DocNo</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox25"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Gate Pass No.</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox23</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox26"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox11</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""DocNo3"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!DocNo.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>DocNo</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox30"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Godown</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox9</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox35"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox10</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""DocNo6"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!GodownName.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>DocNo</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox37"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Date</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox24</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox54"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox19</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""DocDate2"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!DocDate.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <Format>dd/MMM/yy</Format>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>DocDate</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox271"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox271</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox214"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Remark</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox214</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox74"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox10</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""DocNo8"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!Remark.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>DocNo</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox75"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Reference No.</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox24</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox82"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox19</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""DocDate4"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!ReferenceDocNo.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>DocDate</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox219"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Sr</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox219</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox170"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Product</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox168</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox171"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Specification</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox170</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox178"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Qty</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox172</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox179"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Unit</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox174</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox220"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=RowNumber(Nothing)</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox220</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Product2"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!Product.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Product</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Specification"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!Specification.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Specification</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Qty5"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Strings.FormatNumber(Fields!Qty.Value, Fields!DecimalPlaces.Value)</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Qty</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""UnitName5"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!UnitName.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>UnitName</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox221"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox221</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox215"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Total :</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox215</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox187"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox187</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox216"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox216</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox231"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Strings.FormatNumber(Sum(Fields!Qty.Value), Max(Fields!DecimalPlaces.Value))</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox217</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox275"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox275</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.36458in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox6"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox1</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox7"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox2</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox188"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox188</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox28"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox3</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox29"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox4</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox31"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox5</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox276"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox276</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox32"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!SignatoryleftCaption.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox6</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>3</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell />
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox38"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!SignatoryMiddleCaption.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox8</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox40"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!SignatoryRightCaption.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox15</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
              </TablixCells>
            </TablixRow>
          </TablixRows>
        </TablixBody>
        <TablixColumnHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
          </TablixMembers>
        </TablixColumnHierarchy>
        <TablixRowHierarchy>
          <TablixMembers>
            <TablixMember>
              <KeepWithGroup>After</KeepWithGroup>
            </TablixMember>
            <TablixMember>
              <KeepWithGroup>After</KeepWithGroup>
            </TablixMember>
            <TablixMember>
              <KeepWithGroup>After</KeepWithGroup>
            </TablixMember>
            <TablixMember>
              <KeepWithGroup>After</KeepWithGroup>
            </TablixMember>
            <TablixMember>
              <KeepWithGroup>After</KeepWithGroup>
            </TablixMember>
            <TablixMember>
              <KeepWithGroup>After</KeepWithGroup>
            </TablixMember>
            <TablixMember>
              <Group Name=""Details5"" />
            </TablixMember>
            <TablixMember>
              <KeepWithGroup>Before</KeepWithGroup>
            </TablixMember>
            <TablixMember>
              <KeepWithGroup>Before</KeepWithGroup>
            </TablixMember>
            <TablixMember>
              <KeepWithGroup>Before</KeepWithGroup>
            </TablixMember>
          </TablixMembers>
        </TablixRowHierarchy>
        <DataSetName>DsGatePass</DataSetName>
        <Top>5.21667in</Top>
        <Left>0.11617in</Left>
        <Height>2.86403in</Height>
        <Width>7.85586in</Width>
        <ZIndex>9</ZIndex>
        <Visibility>
          <Hidden>=Microsoft.VisualBasic.Interaction.iif(First(Fields!GatePassHeaderId.Value, ""DsMain"") = 0, True, False)</Hidden>
        </Visibility>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
        </Style>
      </Tablix>
    </ReportItems>
    <Height>8.0807in</Height>
    <Style />
  </Body>
  <Width>8.0882in</Width>
  <Page>
    <PageHeader>
      <Height>0.61458in</Height>
      <PrintOnFirstPage>true</PrintOnFirstPage>
      <PrintOnLastPage>true</PrintOnLastPage>
      <ReportItems>
        <Textbox Name=""ReportTitle3"">
          <CanGrow>true</CanGrow>
          <KeepTogether>true</KeepTogether>
          <Paragraphs>
            <Paragraph>
              <TextRuns>
                <TextRun>
                  <Value>=First(Fields!ReportTitle.Value, ""DsMain"")</Value>
                  <Style>
                    <FontFamily>Tahoma</FontFamily>
                    <FontSize>14pt</FontSize>
                    <FontWeight>Bold</FontWeight>
                  </Style>
                </TextRun>
              </TextRuns>
              <Style>
                <TextAlign>Center</TextAlign>
              </Style>
            </Paragraph>
          </Paragraphs>
          <rd:DefaultName>ReportTitle</rd:DefaultName>
          <Top>0.02569in</Top>
          <Left>0.0413in</Left>
          <Height>0.25in</Height>
          <Width>7.95551in</Width>
          <Visibility>
            <Hidden>=iif(Globals!PageNumber=1,True,False)</Hidden>
          </Visibility>
          <Style>
            <Border>
              <Style>None</Style>
            </Border>
            <TopBorder>
              <Color>Black</Color>
              <Style>Solid</Style>
              <Width>1pt</Width>
            </TopBorder>
            <BottomBorder>
              <Color>Black</Color>
              <Style>Solid</Style>
              <Width>1pt</Width>
            </BottomBorder>
            <PaddingLeft>2pt</PaddingLeft>
            <PaddingRight>2pt</PaddingRight>
            <PaddingTop>2pt</PaddingTop>
            <PaddingBottom>2pt</PaddingBottom>
          </Style>
        </Textbox>
        <Textbox Name=""Textbox4"">
          <CanGrow>true</CanGrow>
          <KeepTogether>true</KeepTogether>
          <Paragraphs>
            <Paragraph>
              <TextRuns>
                <TextRun>
                  <Value>=First(Fields!PartyCaption.Value, ""DsMain"")+"" : "" + First(Fields!PartyName.Value, ""DsMain"")</Value>
                  <Style>
                    <FontFamily>Tahoma</FontFamily>
                    <FontSize>9pt</FontSize>
                    <FontWeight>Bold</FontWeight>
                  </Style>
                </TextRun>
              </TextRuns>
              <Style />
            </Paragraph>
          </Paragraphs>
          <rd:DefaultName>Textbox4</rd:DefaultName>
          <Top>0.31597in</Top>
          <Left>0.0538in</Left>
          <Height>0.25in</Height>
          <Width>3.83417in</Width>
          <ZIndex>1</ZIndex>
          <Visibility>
            <Hidden>=iif(Globals!PageNumber=1,True,False)</Hidden>
          </Visibility>
          <Style>
            <Border>
              <Style>None</Style>
            </Border>
            <BottomBorder>
              <Color>Black</Color>
              <Style>Solid</Style>
              <Width>1pt</Width>
            </BottomBorder>
            <PaddingLeft>2pt</PaddingLeft>
            <PaddingRight>2pt</PaddingRight>
            <PaddingTop>2pt</PaddingTop>
            <PaddingBottom>2pt</PaddingBottom>
          </Style>
        </Textbox>
        <Textbox Name=""Textbox8"">
          <CanGrow>true</CanGrow>
          <KeepTogether>true</KeepTogether>
          <Paragraphs>
            <Paragraph>
              <TextRuns>
                <TextRun>
                  <Value>=""Order No : "" + First(Fields!DocNo.Value, ""DsMain"")</Value>
                  <Style>
                    <FontFamily>Tahoma</FontFamily>
                    <FontSize>9pt</FontSize>
                    <FontWeight>Bold</FontWeight>
                  </Style>
                </TextRun>
              </TextRuns>
              <Style />
            </Paragraph>
          </Paragraphs>
          <rd:DefaultName>Textbox4</rd:DefaultName>
          <Top>0.31597in</Top>
          <Left>3.92123in</Left>
          <Height>0.25in</Height>
          <Width>4.086in</Width>
          <ZIndex>2</ZIndex>
          <Visibility>
            <Hidden>=iif(Globals!PageNumber=1,True,False)</Hidden>
          </Visibility>
          <Style>
            <Border>
              <Style>None</Style>
            </Border>
            <BottomBorder>
              <Color>Black</Color>
              <Style>Solid</Style>
              <Width>1pt</Width>
            </BottomBorder>
            <PaddingLeft>2pt</PaddingLeft>
            <PaddingRight>2pt</PaddingRight>
            <PaddingTop>2pt</PaddingTop>
            <PaddingBottom>2pt</PaddingBottom>
          </Style>
        </Textbox>
      </ReportItems>
      <Style>
        <Border>
          <Style>None</Style>
        </Border>
      </Style>
    </PageHeader>
    <PageFooter>
      <Height>0.31766in</Height>
      <PrintOnFirstPage>true</PrintOnFirstPage>
      <PrintOnLastPage>true</PrintOnLastPage>
      <ReportItems>
        <Textbox Name=""Textbox18"">
          <CanGrow>true</CanGrow>
          <KeepTogether>true</KeepTogether>
          <Paragraphs>
            <Paragraph>
              <TextRuns>
                <TextRun>
                  <Value>Page </Value>
                  <Style>
                    <FontFamily>tahoma</FontFamily>
                    <FontSize>8pt</FontSize>
                  </Style>
                </TextRun>
                <TextRun>
                  <Value>=Globals!PageNumber</Value>
                  <Style>
                    <FontFamily>tahoma</FontFamily>
                    <FontSize>8pt</FontSize>
                  </Style>
                </TextRun>
                <TextRun>
                  <Value> of </Value>
                  <Style>
                    <FontFamily>tahoma</FontFamily>
                    <FontSize>8pt</FontSize>
                  </Style>
                </TextRun>
                <TextRun>
                  <Value>=Globals!TotalPages</Value>
                  <Style>
                    <FontFamily>tahoma</FontFamily>
                    <FontSize>8pt</FontSize>
                  </Style>
                </TextRun>
              </TextRuns>
              <Style>
                <TextAlign>Right</TextAlign>
              </Style>
            </Paragraph>
          </Paragraphs>
          <rd:DefaultName>Textbox18</rd:DefaultName>
          <Top>0.03849in</Top>
          <Left>5.3688in</Left>
          <Height>0.22917in</Height>
          <Width>2.57384in</Width>
          <Style>
            <Border>
              <Style>None</Style>
            </Border>
            <PaddingLeft>2pt</PaddingLeft>
            <PaddingRight>2pt</PaddingRight>
            <PaddingTop>2pt</PaddingTop>
            <PaddingBottom>2pt</PaddingBottom>
          </Style>
        </Textbox>
        <Textbox Name=""Textbox19"">
          <CanGrow>true</CanGrow>
          <KeepTogether>true</KeepTogether>
          <Paragraphs>
            <Paragraph>
              <TextRuns>
                <TextRun>
                  <Value>Print Date : </Value>
                  <Style>
                    <FontFamily>tahoma</FontFamily>
                    <FontSize>8pt</FontSize>
                    <Format>dd-MMM-yy hh:mm:ss tt</Format>
                  </Style>
                </TextRun>
                <TextRun>
                  <Value>=Globals!ExecutionTime</Value>
                  <Style>
                    <FontFamily>tahoma</FontFamily>
                    <FontSize>8pt</FontSize>
                    <Format>dd-MMM-yy hh:mm:ss tt</Format>
                  </Style>
                </TextRun>
              </TextRuns>
              <Style>
                <TextAlign>Left</TextAlign>
              </Style>
            </Paragraph>
          </Paragraphs>
          <rd:DefaultName>Textbox19</rd:DefaultName>
          <Top>0.06944in</Top>
          <Left>0.0788in</Left>
          <Height>0.1875in</Height>
          <Width>2.52546in</Width>
          <ZIndex>1</ZIndex>
          <Style>
            <Border>
              <Style>None</Style>
            </Border>
            <PaddingLeft>2pt</PaddingLeft>
            <PaddingRight>2pt</PaddingRight>
            <PaddingTop>2pt</PaddingTop>
            <PaddingBottom>2pt</PaddingBottom>
          </Style>
        </Textbox>
        <Textbox Name=""Textbox98"">
          <CanGrow>true</CanGrow>
          <KeepTogether>true</KeepTogether>
          <Paragraphs>
            <Paragraph>
              <TextRuns>
                <TextRun>
                  <Value>=""Printed By : "" + Parameters!PrintedBy.Value</Value>
                  <Style>
                    <FontFamily>tahoma</FontFamily>
                    <FontSize>9pt</FontSize>
                    <Format>dd-MMM-yy hh:mm:ss tt</Format>
                  </Style>
                </TextRun>
              </TextRuns>
              <Style>
                <TextAlign>Center</TextAlign>
              </Style>
            </Paragraph>
          </Paragraphs>
          <rd:DefaultName>Textbox19</rd:DefaultName>
          <Top>0.06195in</Top>
          <Left>2.85186in</Left>
          <Height>0.1875in</Height>
          <Width>2.12468in</Width>
          <ZIndex>2</ZIndex>
          <Style>
            <Border>
              <Style>None</Style>
            </Border>
            <PaddingLeft>2pt</PaddingLeft>
            <PaddingRight>2pt</PaddingRight>
            <PaddingTop>2pt</PaddingTop>
            <PaddingBottom>2pt</PaddingBottom>
          </Style>
        </Textbox>
      </ReportItems>
      <Style>
        <Border>
          <Style>None</Style>
        </Border>
      </Style>
    </PageFooter>
    <PageHeight>11.69in</PageHeight>
    <PageWidth>8.27in</PageWidth>
    <LeftMargin>0.07in</LeftMargin>
    <TopMargin>0.1in</TopMargin>
    <BottomMargin>0.1in</BottomMargin>
    <Style />
  </Page>
  <AutoRefresh>0</AutoRefresh>
  <DataSources>
    <DataSource Name=""DyeingCancelPrint"">
      <ConnectionProperties>
        <DataProvider>SQL</DataProvider>
        <ConnectString>=Parameters!DatabaseConnectionString.Value</ConnectString>
      </ConnectionProperties>
      <rd:SecurityType>None</rd:SecurityType>
      <rd:DataSourceID>727bb65e-cc51-4d28-a097-a8ea7d087931</rd:DataSourceID>
    </DataSource>
  </DataSources>
  <DataSets>
    <DataSet Name=""DsMain"">
      <Query>
        <DataSourceName>DyeingCancelPrint</DataSourceName>
        <QueryParameters>
          <QueryParameter Name=""@Id"">
            <Value>=Parameters!Id.Value</Value>
          </QueryParameter>
        </QueryParameters>
        <CommandType>StoredProcedure</CommandType>
        <CommandText>Web.StdDocPrint_StockIssue</CommandText>
      </Query>
      <Fields>
        <Field Name=""StockHeaderId"">
          <DataField>StockHeaderId</DataField>
          <rd:TypeName>System.Int32</rd:TypeName>
        </Field>
        <Field Name=""DocTypeId"">
          <DataField>DocTypeId</DataField>
          <rd:TypeName>System.Int32</rd:TypeName>
        </Field>
        <Field Name=""DocNo"">
          <DataField>DocNo</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""DocIdCaption"">
          <DataField>DocIdCaption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""SiteId"">
          <DataField>SiteId</DataField>
          <rd:TypeName>System.Int32</rd:TypeName>
        </Field>
        <Field Name=""DivisionId"">
          <DataField>DivisionId</DataField>
          <rd:TypeName>System.Int32</rd:TypeName>
        </Field>
        <Field Name=""DocDate"">
          <DataField>DocDate</DataField>
          <rd:TypeName>System.DateTime</rd:TypeName>
        </Field>
        <Field Name=""DocIdCaptionDate"">
          <DataField>DocIdCaptionDate</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""ProcessName"">
          <DataField>ProcessName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Remark"">
          <DataField>Remark</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""DocumentTypeShortName"">
          <DataField>DocumentTypeShortName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""GatePassHeaderId"">
          <DataField>GatePassHeaderId</DataField>
          <rd:TypeName>System.Int32</rd:TypeName>
        </Field>
        <Field Name=""ModifiedBy"">
          <DataField>ModifiedBy</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""ModifiedDate"">
          <DataField>ModifiedDate</DataField>
          <rd:TypeName>System.DateTime</rd:TypeName>
        </Field>
        <Field Name=""Status"">
          <DataField>Status</DataField>
          <rd:TypeName>System.Int32</rd:TypeName>
        </Field>
        <Field Name=""ReverseCharge"">
          <DataField>ReverseCharge</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""CompanyName"">
          <DataField>CompanyName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""GodownName"">
          <DataField>GodownName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""FromGodownName"">
          <DataField>FromGodownName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""PartyName"">
          <DataField>PartyName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""PartyCaption"">
          <DataField>PartyCaption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""PartySuffix"">
          <DataField>PartySuffix</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""PartyAddress"">
          <DataField>PartyAddress</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""PartyStateName"">
          <DataField>PartyStateName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""PartyStateCode"">
          <DataField>PartyStateCode</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""PartyMobileNo"">
          <DataField>PartyMobileNo</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""CustomerId"">
          <DataField>CustomerId</DataField>
          <rd:TypeName>System.Int32</rd:TypeName>
        </Field>
        <Field Name=""Party_TIN_NO"">
          <DataField>Party TIN NO</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Party_PAN_NO"">
          <DataField>Party PAN NO</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Party_AADHAR_NO"">
          <DataField>Party AADHAR NO</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Party_GST_NO"">
          <DataField>Party GST NO</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Party_CST_NO"">
          <DataField>Party CST NO</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""SignatoryMiddleCaption"">
          <DataField>SignatoryMiddleCaption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""SignatoryRightCaption"">
          <DataField>SignatoryRightCaption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""ProductName"">
          <DataField>ProductName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""ProductCaption"">
          <DataField>ProductCaption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""UnitName"">
          <DataField>UnitName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""DecimalPlaces"">
          <DataField>DecimalPlaces</DataField>
          <rd:TypeName>System.Byte</rd:TypeName>
        </Field>
        <Field Name=""Qty"">
          <DataField>Qty</DataField>
          <rd:TypeName>System.Decimal</rd:TypeName>
        </Field>
        <Field Name=""Rate"">
          <DataField>Rate</DataField>
          <rd:TypeName>System.Decimal</rd:TypeName>
        </Field>
        <Field Name=""Amount"">
          <DataField>Amount</DataField>
          <rd:TypeName>System.Decimal</rd:TypeName>
        </Field>
        <Field Name=""Dimension1Name"">
          <DataField>Dimension1Name</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Dimension1Caption"">
          <DataField>Dimension1Caption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Dimension2Name"">
          <DataField>Dimension2Name</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Dimension2Caption"">
          <DataField>Dimension2Caption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Dimension3Name"">
          <DataField>Dimension3Name</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Dimension3Caption"">
          <DataField>Dimension3Caption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Dimension4Name"">
          <DataField>Dimension4Name</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Dimension4Caption"">
          <DataField>Dimension4Caption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""LotNo"">
          <DataField>LotNo</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Specification"">
          <DataField>Specification</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""SpecificationCaption"">
          <DataField>SpecificationCaption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""SignatoryleftCaption"">
          <DataField>SignatoryleftCaption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""LineRemark"">
          <DataField>LineRemark</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""SalesTaxProductCodes"">
          <DataField>SalesTaxProductCodes</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""ProductGroupName"">
          <DataField>ProductGroupName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""ProductGroupCaption"">
          <DataField>ProductGroupCaption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""ProductUidName"">
          <DataField>ProductUidName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""CostCenterName"">
          <DataField>CostCenterName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""CostCenterCaption"">
          <DataField>CostCenterCaption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""ProductUidCaption"">
          <DataField>ProductUidCaption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""CostCenterCnt"">
          <DataField>CostCenterCnt</DataField>
          <rd:TypeName>System.Int32</rd:TypeName>
        </Field>
        <Field Name=""SubReportProcList"">
          <DataField>SubReportProcList</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""ReportTitle"">
          <DataField>ReportTitle</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""ReportName"">
          <DataField>ReportName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""SalesTaxGroupProductCaption"">
          <DataField>SalesTaxGroupProductCaption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""SalesTaxProductCodeCaption"">
          <DataField>SalesTaxProductCodeCaption</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
      </Fields>
    </DataSet>
    <DataSet Name=""DsGatePass"">
      <Query>
        <DataSourceName>DyeingCancelPrint</DataSourceName>
        <CommandText />
      </Query>
      <Fields>
        <Field Name=""GatePassHeaderId"">
          <DataField>GatePassHeaderId</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""DocNo"">
          <DataField>DocNo</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""DocDate"">
          <DataField>DocDate</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""Remark"">
          <DataField>Remark</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""PersonName"">
          <DataField>PersonName</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""GodownName"">
          <DataField>GodownName</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""GatePassLineId"">
          <DataField>GatePassLineId</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""Product"">
          <DataField>Product</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""Specification"">
          <DataField>Specification</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""Qty"">
          <DataField>Qty</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""UnitName"">
          <DataField>UnitName</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""DecimalPlaces"">
          <DataField>DecimalPlaces</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""ReferenceDocNo"">
          <DataField>ReferenceDocNo</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""SignatoryleftCaption"">
          <DataField>SignatoryleftCaption</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""SignatoryMiddleCaption"">
          <DataField>SignatoryMiddleCaption</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""SignatoryRightCaption"">
          <DataField>SignatoryRightCaption</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""ReportTitle"">
          <DataField>ReportTitle</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
      </Fields>
    </DataSet>
  </DataSets>
  <ReportParameters>
    <ReportParameter Name=""Id"">
      <DataType>String</DataType>
      <Nullable>true</Nullable>
      <AllowBlank>true</AllowBlank>
      <Prompt>Id</Prompt>
    </ReportParameter>
    <ReportParameter Name=""PrintedBy"">
      <DataType>String</DataType>
      <Nullable>true</Nullable>
      <AllowBlank>true</AllowBlank>
      <Prompt>PrintedBy</Prompt>
    </ReportParameter>
    <ReportParameter Name=""DatabaseConnectionString"">
      <DataType>String</DataType>
      <Nullable>true</Nullable>
      <AllowBlank>true</AllowBlank>
      <Prompt>DatabaseConnectionString</Prompt>
    </ReportParameter>
    <ReportParameter Name=""CompanyName"">
      <DataType>String</DataType>
      <Nullable>true</Nullable>
      <AllowBlank>true</AllowBlank>
      <Prompt>CompanyName</Prompt>
    </ReportParameter>
  </ReportParameters>
  <Code>
    Function RupeesToWord(ByVal MyNumber)
    Dim Temp
    Dim Rupees, Paisa As String
    Dim DecimalPlace, iCount
    Dim Hundreds, Words As String
    Dim place(9) As String
    place(0) = "" Thousand ""
    place(2) = "" Lakh ""
    place(4) = "" Crore ""
    place(6) = "" Arab ""
    place(8) = "" Kharab ""
    On Error Resume Next
    ' Convert MyNumber to a string, trimming extra spaces.
    MyNumber = Trim(Str(MyNumber))

    ' Find decimal place.
    DecimalPlace = InStr(MyNumber, ""."") 

    ' If we find decimal place...
    If DecimalPlace &gt; 0 Then
    ' Convert Paisa
    Temp = Left(Mid(MyNumber, DecimalPlace + 1) &amp; ""00"", 2)
    'Paisa = "" and "" &amp; ConvertTens(Temp) &amp; "" Paisa""
    Paisa = """"

    ' Strip off paisa from remainder to convert.
    MyNumber = Trim(Left(MyNumber, DecimalPlace - 1))
    End If

    '===============================================================
    Dim TM As String ' If MyNumber between Rs.1 To 99 Only.
    TM = Right(MyNumber, 2)

    If Len(MyNumber) &gt; 0 And Len(MyNumber) &lt;= 2 Then
    If Len(TM) = 1 Then
    Words = ConvertDigit(TM)
    RupeesToWord = Words &amp; Paisa

    Exit Function

    Else
    If Len(TM) = 2 Then
    Words = ConvertTens(TM)
    RupeesToWord = Words &amp; Paisa
    Exit Function

    End If
    End If
    End If
    '===============================================================


    ' Convert last 3 digits of MyNumber to ruppees in word.
    Hundreds = ConvertHundreds(Right(MyNumber, 3))
    ' Strip off last three digits
    MyNumber = Left(MyNumber, Len(MyNumber) - 3)

    iCount = 0
    Do While MyNumber &lt;&gt; """"
    'Strip last two digits
    Temp = Right(MyNumber, 2)
    If Len(MyNumber) = 1 Then


    If Trim(Words) = ""Thousand"" Or _
    Trim(Words) = ""Lakh Thousand"" Or _
    Trim(Words) = ""Lakh"" Or _
    Trim(Words) = ""Crore"" Or _
    Trim(Words) = ""Crore Lakh Thousand"" Or _
    Trim(Words) = ""Arab Crore Lakh Thousand"" Or _
    Trim(Words) = ""Arab"" Or _
    Trim(Words) = ""Kharab Arab Crore Lakh Thousand"" Or _
    Trim(Words) = ""Kharab"" Then

    Words = ConvertDigit(Temp) &amp; place(iCount)
    MyNumber = Left(MyNumber, Len(MyNumber) - 1)

    Else

    Words = ConvertDigit(Temp) &amp; place(iCount) &amp; Words
    MyNumber = Left(MyNumber, Len(MyNumber) - 1)

    End If
    Else

    If Trim(Words) = ""Thousand"" Or _
    Trim(Words) = ""Lakh Thousand"" Or _
    Trim(Words) = ""Lakh"" Or _
    Trim(Words) = ""Crore"" Or _
    Trim(Words) = ""Crore Lakh Thousand"" Or _
    Trim(Words) = ""Arab Crore Lakh Thousand"" Or _
    Trim(Words) = ""Arab"" Then


    Words = ConvertTens(Temp) &amp; place(iCount)


    MyNumber = Left(MyNumber, Len(MyNumber) - 2)
    Else

    '=================================================================
    ' if only Lakh, Crore, Arab, Kharab

    If Trim(ConvertTens(Temp) &amp; place(iCount)) = ""Lakh"" Or _
    Trim(ConvertTens(Temp) &amp; place(iCount)) = ""Crore"" Or _
    Trim(ConvertTens(Temp) &amp; place(iCount)) = ""Arab"" Then

    Words = Words
    MyNumber = Left(MyNumber, Len(MyNumber) - 2)
    Else
    Words = ConvertTens(Temp) &amp; place(iCount) &amp; Words
    MyNumber = Left(MyNumber, Len(MyNumber) - 2)
    End If

    End If
    End If

    iCount = iCount + 2
    Loop

    RupeesToWord = Words &amp; Hundreds &amp; Paisa
    End Function

    ' Conversion for hundreds
    '*****************************************
    Private Function ConvertHundreds(ByVal MyNumber)
    Dim Result As String

    ' Exit if there is nothing to convert.
    If Val(MyNumber) = 0 Then Exit Function

    ' Append leading zeros to number.
    MyNumber = Right(""000"" &amp; MyNumber, 3)

    ' Do we have a hundreds place digit to convert?
    If Left(MyNumber, 1) &lt;&gt; ""0"" Then
    Result = ConvertDigit(Left(MyNumber, 1)) &amp; "" Hundred ""
    End If

    ' Do we have a tens place digit to convert?
    If Mid(MyNumber, 2, 1) &lt;&gt; ""0"" Then
    Result = Result &amp; ConvertTens(Mid(MyNumber, 2))
    Else
    ' If not, then convert the ones place digit.
    Result = Result &amp; ConvertDigit(Mid(MyNumber, 3))
    End If

    ConvertHundreds = Trim(Result)
    End Function

    ' Conversion for tens
    '*****************************************
    Private Function ConvertTens(ByVal MyTens)
    Dim Result As String

    ' Is value between 10 and 19?
    If Val(Left(MyTens, 1)) = 1 Then
    Select Case Val(MyTens)
    Case 10 : Result = ""Ten""
    Case 11 : Result = ""Eleven""
    Case 12 : Result = ""Twelve""
    Case 13 : Result = ""Thirteen""
    Case 14 : Result = ""Fourteen""
    Case 15 : Result = ""Fifteen""
    Case 16 : Result = ""Sixteen""
    Case 17 : Result = ""Seventeen""
    Case 18 : Result = ""Eighteen""
    Case 19 : Result = ""Nineteen""
    Case Else
    End Select
    Else
    ' .. otherwise it's between 20 and 99.
    Select Case Val(Left(MyTens, 1))
    Case 2 : Result = ""Twenty ""
    Case 3 : Result = ""Thirty ""
    Case 4 : Result = ""Forty ""
    Case 5 : Result = ""Fifty ""
    Case 6 : Result = ""Sixty ""
    Case 7 : Result = ""Seventy ""
    Case 8 : Result = ""Eighty ""
    Case 9 : Result = ""Ninety ""
    Case Else
    End Select

    ' Convert ones place digit.
    Result = Result &amp; ConvertDigit(Right(MyTens, 1))
    End If

    ConvertTens = Result
    End Function

    Private Function ConvertDigit(ByVal MyDigit)
    Select Case Val(MyDigit)
    Case 1 : ConvertDigit = ""One""
    Case 2 : ConvertDigit = ""Two""
    Case 3 : ConvertDigit = ""Three""
    Case 4 : ConvertDigit = ""Four""
    Case 5 : ConvertDigit = ""Five""
    Case 6 : ConvertDigit = ""Six""
    Case 7 : ConvertDigit = ""Seven""
    Case 8 : ConvertDigit = ""Eight""
    Case 9 : ConvertDigit = ""Nine""
    Case Else : ConvertDigit = """"
    End Select
    End Function



    Public Function GenerateVbCrLf(ByVal Count as integer)
    dim Value as String
    dim i as integer
    For i = 1 To Count Step 1
    Value = Value &amp; "" "" &amp; VbCrLf
    Next i
    Return Value
    End Function

Public Function PrintProduct(ProductName as String,ProductGroupCaption as String,ProductGroupName as String,SpecificationCaption as String,Specification as String,Dimension1Caption as String,Dimension1Name as String,Dimension2Caption as String,Dimension2Name as String,Dimension3Caption as String,Dimension3Name as String,Dimension4Caption as String,Dimension4Name as String,ProductUidCaption as String,ProductUidName as String)
dim str as String
Dim C As Integer = 0
  str =iif(ProductName  &lt;&gt; """",ProductName+""&lt;BR&gt;""  ,"""")
  if ProductGroupName &lt;&gt; """"  Then 
     str =str +""&lt;b&gt;""+ProductGroupCaption+""&lt;/b&gt;""+"" : ""+ ProductGroupName
  C=C+1
 End If


if Specification &lt;&gt; """"  Then
str =str +"" ""+iif(C mod 2=1,"" , "","""")    
  C=C+1
     str =str +""&lt;Br&gt;""+""&lt;b&gt;""+SpecificationCaption+""&lt;/b&gt;"" +""  : ""+ Specification+iif(C mod 2=0,""&lt;BR&gt;"","""")  
 End If

  if Dimension1Name &lt;&gt; """"  Then
    str =str +"" ""+iif(C mod 2=1,"" , "","""")    
     C=C+1
     str =str +"" ""+""&lt;b&gt;""+Dimension1Caption+""&lt;/b&gt;"" + "" : ""+ Dimension1Name +iif(C mod 2=0,""&lt;BR&gt;"","""")    
 End If      

  if Dimension2Name &lt;&gt; """"  Then
   str =str +"" ""+iif(C mod 2=1,"" , "","""")    
     C=C+1
     str =str +""&lt;b&gt;""+Dimension2Caption+""&lt;/b&gt;"" +"" : ""+ Dimension2Name +iif(C mod 2=0,""&lt;BR&gt;"","""")    
 End If   

  if Dimension3Name &lt;&gt; """"  Then
  str =str +"" ""+iif(C mod 2=1,"" , "","""")    
     C=C+1
     str =str +""&lt;b&gt;""+Dimension3Caption+""&lt;/b&gt;"" +"" : ""+ Dimension3Name +iif(C mod 2=0,""&lt;BR&gt;"","""")    
 End If   

  if Dimension4Name &lt;&gt; """"  Then
   str =str +"" ""+iif(C mod 2=1,"" , "","""")    
     C=C+1
     str =str + ""&lt;b&gt;""+Dimension4Caption+""&lt;/b&gt;"" +"" : ""+ Dimension4Name +iif(C mod 2=0,""&lt;BR&gt;"","""")    
 End If  

  if ProductUidName &lt;&gt; """"  Then
    str =str +"" ""+iif(C mod 2=1,"" , "","""")    
     C=C+1
       str =str +"" ""+""&lt;b&gt;""+ProductUidCaption +""&lt;/b&gt;""+"" : ""+ ProductUidName+iif(C mod 2=0,""&lt;BR&gt;"","""")    
 End If   
 
if Right(str, 4).toupper()=""&lt;BR&gt;"" Then
	str =str.Substring(0,str.Length-4)
End If
return str 
End Function



Public Function PrintLineRemark(Remark as String,Lot as String,DiscountPer as String,DiscountAmt as String)
dim str as String
Dim C As Integer = 0
  str =iif(Remark &lt;&gt; """",Remark ,"""")

  if Lot &lt;&gt; """"  Then 
     str =str +""&lt;BR&gt;""+""&lt;b&gt;""+""Lot""+""&lt;/b&gt;""+"" : ""+ Lot 
     C=C+1 
 End If

if DiscountPer &lt;&gt; """"  Then
    str =str +"" ""+iif(C mod 2=1,"" , "","""")    
     C=C+1
       str =str +"" ""+""&lt;b&gt;""+""Dis Per"" +""&lt;/b&gt;""+"" : ""+ DiscountPer +iif(C mod 2=0,""&lt;BR&gt;"","""")    
 End If  

if DiscountAmt &lt;&gt; """"  Then
    str =str +"" ""+iif(C mod 2=1,"" , "","""")    
     C=C+1
       str =str +"" ""+""&lt;b&gt;""+""Dis Amt"" +""&lt;/b&gt;""+"" : ""+ DiscountAmt +iif(C mod 2=0,""&lt;BR&gt;"","""")    
 End If

if Right(str, 4).toupper()=""&lt;BR&gt;"" Then
	str =str.Substring(0,str.Length-4)
End If

return str 
End Function
</Code>
  <EmbeddedImages>
    <EmbeddedImage Name=""SuryaLogo"">
      <MIMEType>image/jpeg</MIMEType>
      <ImageData>/9j/4AAQSkZJRgABAQAAAQABAAD/2wCEAAkGBwgHBgkIBwgKCgkLDRYPDQwMDRsUFRAWIB0iIiAdHx8kKDQsJCYxJx8fLT0tMTU3Ojo6Iys/RD84QzQ5OjcBCgoKDQwNGg8PGjclHyU3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3N//AABEIAGEAYAMBEQACEQEDEQH/xAAbAAACAwEBAQAAAAAAAAAAAAADBAABBQIGB//EADkQAAEDAwIDBgQDBgcAAAAAAAECAwQABRESITFBUQYTFGFxgRUikaEysfAjM0JSc4IkNDVjcsHx/8QAGQEAAwEBAQAAAAAAAAAAAAAAAAIDBAEF/8QAKhEAAgICAQQBAwQDAQAAAAAAAQIAAxESBBMhMUEiUXGBFDNhoSMywbH/2gAMAwEAAhEDEQA/APuNEJKISlKCeJArhOIScq7CIQU3AS5RmraVHKh3AQNwPP7VCsW7tvjHqVc16jTz7l3oR1WyQmXI8MwU4U9qxp96pZV1VKfWTW0VMHPqEtXc/Do4iv8AiGQgBDurVqHXNCV9NQn0gbBYdx7gpSLgq4xlRnW0xAD3yFD5j0xU3FpsBU/H3HQ16HI7+o/wFXk5SFpWkKQoKB5g5rgIPiE6rsJKISUQlZohErrbWbpGDEgrCAsL+RWDkVG6lbl1aUqtaptlilj7RQbw/JjQ+8Co2x1pwFDOMj6VoNZQCZKuQlpIX1FblZXXO0kW7m5lhhkAKZJwDjOwOcb53oNqIh2nG47PcHB8eo/fo8Cba3GLkoiM5jJScHIORipNetI3Y9pZqesNCJ3ZWoMO1NM29f8AhWwcKUd+OTnzzXF5CWjqA9p1aekOmB4mbZrGuLfpl1FyMhmTnS2NwMnO5zjbgKt1VdAFkEoau1nJ8w987QwbbNjW+Wl0rljGUDZIJ07+/SgVF1M7ZyVqcKfJmha7exbIojRQoNgk
/Mcnes9FK0povia7bWtbZvMcq0nJRCUaITPkwSu5MzvEvJDKCCyk/Kr2/XAVBqtrQ+3j1KC0LWUx59xPs12jj9oUSTHZda7hQB7zG4OcHb0O1anQpiY6OQt2cDxObk41ZVg26Cwl6UoqcUlvGs+eOJ3rzudzHp1CjJM2cfj1sSfENLgPXdiKt5RjKTkrbxnj+vvU7uM/LRC/x/iOlgqJx3jHweMqC1EdLi0NHKSVYP29at+jraoVtkgReqwYsJSrPGEFcNsuIbWrUSDk59/Sg8OsVGpSQDDrNtsYvFgv2iJJMYmStRBQgjGPv+sVKrjvxKm0+X0jPYLWGe05tLqLsouT4bJfjL+Ram90k9M7g7U/B5dlysGGCIl9KAg+Zx2m7St9n1xEORXX/EKI+Q40gY+p34V6KJsD3mO/kCkgYzmPxLcli4yZoddUqQBlCj8qcdKypTpYz5PebGtLIFx4mhV5OIXCZJjSIrbENb6Hl6XFpOzY23P3+lQttdGUKuc/1K1orAknGJSbzAVdFWwSUGYkZLW+evHhnG+K06HG3qZuqm+me8FeGXWYCxbGwhal5X3ScE54nbnwrDzjaav8XmaKBWH+UPa4zzURtMxZdeG+VblPlmn41TpWBYcmFjAsdR2j1aZOSiElEJWKIRG7RXZERaIjhaczq+XbV5E1l5VTvWRWcGUrYK2Wg7O0+YLYuCcuJUSjvBkpHL3rnCFopAt8wu0LfHxIb5bxePhPiB4zGdGD0zjPDON8Vt0bG3qZusm/Tz3hLbImvuyRMi9whDmGjqzrT1/XWs1L2MW3XH0mqxUUDQ5jxNXkpnIsluRdlXVLA8YoYLmo9MZxwzjbNMXOuskKUD9THeJ2mJNTc5D8xTmBkJyrZWTy8hXkcSm4XM9pP/JttdNAFE3civVmeXRCVmiEmaISZohJmiEw7rAluXViREUvBxqOrZGP+scq8vlce5uQrVmaK7FFZVo98It5uYuZjI8ZjHe8+GPT
ONs16uxxiY+km++O8eyKWUmP8Jkx7ZPYiT3vESNRbdcP7skbYqXHq6OcknJjchzauFGDiddmYU+32pDF1leJkBROvUVYHIZO5rS5DHImehHRMOcmJQo0S6SZyLitapyH1gN98pKmm8kIKACMApwcjiSa6SVAx4k1VXYhj3+8dfSWbtaWkuLUkNuglSslWEp3PU0vkGUIw6j7zUe/dL/4n8qWVPieXhNJmRezbUkrWhcIqWNahqIQjc4O/GqnttiZUGy15+kcbjNx7wq3MLdVEfirW8yp1Su6OQAQScp1Aq2z/DtS57Zj4xZoPBE6iXE26PIiXNxSnIaQpDhGVPtHZJ81Z+Ujr6igjPiCvoCrev7hGVu262yrlcNSpDiS6toHIR/K2n7DzJNcPc4E6CUQu3mB7Ph+DIdt01wrdWgSkqJ4lX7wD0Xv6LFdbuMzlOVJU/eH7T26XdLWqNBlmM6VpVqyRkDlkb0IwU5Ma9GsTVTgyvgpkW6BGmy3nHYukqdSrBcUBxNZ+RStxBPbBzNHGdqUx57YhHL5D+HzZjClPIhhRcSlJByOQzXabUuPwM5erUrsw/mV2cvKL7bBNQypkaykpUc8Oh5irumpxIU3C1NsYnU1i2XOCJbi21NJSVtykLwWx/MlY4VwEgwcI67TNZlOJb7PzrgrTqQpDjihgBS0jST0zj6mm+oEmCRozTbuUtmHAekPrCW0oJyee2wHUnpSAd5d2CrkzBbt6VJ7OwpqFZaiKSpIUU4UEIHEVTP+xEgE/bVvpG7a03Z7i5AKcNSlFyO6dyo/xIUo8SOIzy9KU/IZjIBW2p9wt5ZbcudlUtAURKVgkf7az+aQfYUKexnbQC6ff/hlXhCp86LbUOLbSn/EurSASAk/INwRurf+00L2GYWjdgv5i90ivQCxdVzZEgRF5cS4EAd0rAXwSOAwr+2gHPbEWxSmLM5xG+0l4+B2pU4MF/CgnSDgDPMnkK4i7NiNfb0k2AzKTf4y
LbCmyUOMiWE6UFOSkkc6hfclBAb2cTTx0a9cqPWZpNR2WQoNNIQFnUrSkDJ6mmVVXwMThJPmcNvRW3vCNuMpdCdQZSQCB1xT98ZiZUHUTztuats28SEybbFS4VFSP2YyVA/xDgT515/G572XNU34lbOJWqhwJ6d1lt5tTbraVtqGFIUMgj0r0MyZAIwYlGsdrjOpdYgMIWj8BCPwenT2ptjEFKDwI4phpTzbqkJLjeQhRG6c8cfSlzHx3zJIjMyAgPNpWELC06h+FQ4EedGYFQfMjjDTi21uNpUppWpskfhOCMj2JHvXcwIBkSw0h9b4bSHVgBS8bkDOB9zXIYGczGvdyU1NahtoS6lYw82oZ1BW2PpmvN5XMeq9UQfeaEpD1ktNNEmC0+i3h9lL4QNLGsatI8vavTwfMy7KDpmMKQheNSQrByMjgaUgHzHH8RO5G4h2N8PDRQXP2/eck+X3qNxtyvT/ADKV9PB3/ETHZuIO0RvfeO+Ix+DI0506c9eFauoddZj/AE69Xq+4xeXDCiLlRmUd9kAr07gda8/mOaazYg7zbUN21Y9oa1ThPiB3SUqB0qGNs+VPxb+vWGnLE0bEcrVJy6ISUQkohErrN8FDU8EFR4JGNs+flWfk39GstiPWuzYgLI87MiJfloT3oUQlenBI6/nUuE7XV72DvGuUI2FME52dgrvyL0e88SkcAr5ScYzj0r0NzrrMZ46Gzqe41bkXBEiUZzjSmSv9gEDcJ8/tWWoWhm3Pb1NdhrwNPzHsVeTicm5Ro8xiG44Uvv57saSfvUWvRXCHyY4qdkLDwJl9mLLcLUuabjcDMS8sFAJJxxyTngTngOlabGVgMCY+PVZWWLNnMZvjExxDJt6iC0okpSrHpXmc2q4qpp9TfS6ZO86Xcl2+LHNySS85kK7sZx+tq63KNFa9Ydz9ICrdjp4jSLnEXFTJ74JaUdIUoY36VdeVS1Ys27GIa2219yKuUQRlyUvBbSDhRQM4
PtQeVUENmewh02zjEWZuZuLElNvBS6gDSXQMHP8A5UE5f6lWFPkR2q6ZG8HZI8xgvruK93SMJUsHJ61zg03qGN3uFzocawHaqxS70mIIc9UTuHNSgM/Nw32PEcvWvTrcL5EwcilrcatjE0Y1yYdnu25KlmQwkFeU7Hhz9xWZb0aw1jyJtNLrWHPgx+rSclEINTLSnEuKbQVo/CojcehrhUE5InQSBgQcyWxCZL0p1LTYIGpVLZYta7McCdRGc4UZmL2f7PG2XOdcPHLkIlnUEkcATnJOd+lWNgdRiZKuOanYk+ZL5erazdolnmxnHVyCkpUBsgqOBzzx6VJ+Kt6fIZAjtyhVYE9mP3NiA1a1JllLEVoaioHGjzqL8Wu2vpY7TR1jWeoTKs7Nudtg8EtMiM7klZ31ct/pRXxEpQ14nOv1fmDELRercb7JskOK404zkqXj5VEYz58+dXTjLSmUGAZAcsW2ms+RO+0fZtN6lwpK5jjHhFasJGc7g5HQ7caotmgMW7j9Vgc4xNaBNjT2S7EdDiAopJHWoV2paNkOZretqzhhGA2gKKwkBR4nG5p8DOYk6rsJKISUQgZcViY0WpLSXWyQSlQyKR0VxhhkRlZlOVhEpCEBKQAAMADlTDsIvmZjKzKu7yJFtCRGA7mSsA5zx0nG3tUq7XNjIRgD3HepNVfOTD3lmG/bJDdyTmLoy5x4DflVTYKxufUmaur8D7g7AxBYtMdFrSRFwSjVnPHfOeea4louUWDwYCjof4/pOZi3ItxjmLbg6ZB0vPpABQBwyf1wqdtrqyqBkH+pSupCGYnB/wDZpAZG9ViQcaMxFR3cZlDSMk6UJwMmuKiqMKMRmZmOWMNTRZKISUQkohJRCSiE4HEV2cgbl/kZH9M/lUb/ANtvtKV/7iCsv+lxv6YpeL+yv2jX/uNG1cavIGdDhROiXROyUQkohP/Z</ImageData>
    </EmbeddedImage>
  </EmbeddedImages>
  <rd:ReportUnitType>Inch</rd:ReportUnitType>
  <rd:ReportID>f1b70379-da78-4ea9-bab5-cbb93d10df58</rd:ReportID>
</Report>";

            return StringCode;

        }

        public string Create_Std_StockIssue_Print_Lanscape()
        {
            string StringCode = "";
            StringCode = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Report xmlns=""http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition"" xmlns:rd=""http://schemas.microsoft.com/SQLServer/reporting/reportdesigner"">
  <Body>
    <ReportItems>
      <Subreport Name=""CompanyDetail_HalfLandscap"">
        <ReportName>CompanyDetail_HalfLandscap</ReportName>
        <Top>0.03271in</Top>
        <Left>0.06627in</Left>
        <Height>0.3in</Height>
        <Width>5.4in</Width>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
        </Style>
      </Subreport>
      <Textbox Name=""ReportTitle3"">
        <CanGrow>true</CanGrow>
        <KeepTogether>true</KeepTogether>
        <Paragraphs>
          <Paragraph>
            <TextRuns>
              <TextRun>
                <Value>=First(Fields!ReportTitle.Value, ""DsMain"")</Value>
                <Style>
                  <FontFamily>Tahoma</FontFamily>
                  <FontSize>14pt</FontSize>
                  <FontWeight>Bold</FontWeight>
                </Style>
              </TextRun>
            </TextRuns>
            <Style>
              <TextAlign>Center</TextAlign>
            </Style>
          </Paragraph>
        </Paragraphs>
        <rd:DefaultName>ReportTitle</rd:DefaultName>
        <Top>0.36744in</Top>
        <Left>0.06626in</Left>
        <Height>0.25in</Height>
        <Width>5.40001in</Width>
        <ZIndex>1</ZIndex>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
          <TopBorder>
            <Style>Solid</Style>
          </TopBorder>
          <BottomBorder>
            <Style>Solid</Style>
          </BottomBorder>
          <PaddingLeft>2pt</PaddingLeft>
          <PaddingRight>2pt</PaddingRight>
          <PaddingTop>2pt</PaddingTop>
          <PaddingBottom>2pt</PaddingBottom>
        </Style>
      </Textbox>
      <Subreport Name=""CompanyDetail_HalfLandscap2"">
        <ReportName>CompanyDetail_HalfLandscap</ReportName>
        <Top>0.02112in</Top>
        <Left>5.67751in</Left>
        <Height>0.3in</Height>
        <Width>5.4in</Width>
        <ZIndex>2</ZIndex>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
        </Style>
      </Subreport>
      <Line Name=""Line2"">
        <Top>0.03271in</Top>
        <Left>5.5691in</Left>
        <Height>3.27813in</Height>
        <Width>0in</Width>
        <ZIndex>3</ZIndex>
        <Style>
          <Border>
            <Style>Solid</Style>
          </Border>
        </Style>
      </Line>
      <Textbox Name=""ReportTitle4"">
        <CanGrow>true</CanGrow>
        <KeepTogether>true</KeepTogether>
        <Paragraphs>
          <Paragraph>
            <TextRuns>
              <TextRun>
                <Value>=First(Fields!ReportTitle.Value, ""DsMain"")</Value>
                <Style>
                  <FontFamily>Tahoma</FontFamily>
                  <FontSize>14pt</FontSize>
                  <FontWeight>Bold</FontWeight>
                </Style>
              </TextRun>
            </TextRuns>
            <Style>
              <TextAlign>Center</TextAlign>
            </Style>
          </Paragraph>
        </Paragraphs>
        <rd:DefaultName>ReportTitle</rd:DefaultName>
        <Top>0.35494in</Top>
        <Left>5.67751in</Left>
        <Height>0.25in</Height>
        <Width>5.4in</Width>
        <ZIndex>4</ZIndex>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
          <TopBorder>
            <Style>Solid</Style>
          </TopBorder>
          <BottomBorder>
            <Style>Solid</Style>
          </BottomBorder>
          <PaddingLeft>2pt</PaddingLeft>
          <PaddingRight>2pt</PaddingRight>
          <PaddingTop>2pt</PaddingTop>
          <PaddingBottom>2pt</PaddingBottom>
        </Style>
      </Textbox>
      <Tablix Name=""Tablix10"">
        <TablixBody>
          <TablixColumns>
            <TablixColumn>
              <Width>0.84334in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.21744in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>1.91219in</Width>
            </TablixColumn>
          </TablixColumns>
          <TablixRows>
            <TablixRow>
              <Height>0.2in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox38"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!PartyCaption.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox26</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                          <Width>1pt</Width>
                        </TopBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""BuyerName3"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Format>0.0000;(0.0000);'-'</Format>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>BuyerName</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                          <Width>1pt</Width>
                        </TopBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Jobworker2"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!PartyName.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Jobworker</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                          <Width>1pt</Width>
                        </TopBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.21042in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox41"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox3</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                          <Width>1pt</Width>
                        </TopBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox117"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox117</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                          <Width>1pt</Width>
                        </TopBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox135"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!PartySuffix.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox134</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                          <Width>1pt</Width>
                        </TopBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.22083in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox198"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Address</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox177</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox78"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox60</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Address3"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!PartyAddress.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Address</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.23958in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox253"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Mobile No.</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox252</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox87"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox60</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""MobileNo3"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!PartyMobileNo.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>MobileNo</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox151"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>GSTIN</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox146</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox144"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox143</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox152"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!Party_GST_NO.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox145</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox119"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value EvaluationMode=""Constant"">Aadhar No</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox115</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox80"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox60</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""TinNo3"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!Party_AADHAR_NO.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>TinNo</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox153"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value EvaluationMode=""Constant"">PAN No</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox147</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox154"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox148</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""AADHAR2"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!Party_PAN_NO.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>AADHAR</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
          </TablixRows>
        </TablixBody>
        <TablixColumnHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember />
            <TablixMember />
          </TablixMembers>
        </TablixColumnHierarchy>
        <TablixRowHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember>
              <Visibility>
                <Hidden>=iif(max(fields!Party_GST_NO.Value) &lt;&gt; """",False,True)</Hidden>
              </Visibility>
            </TablixMember>
            <TablixMember>
              <Visibility>
                <Hidden>=iif(max(fields!Party_AADHAR_NO.Value) &lt;&gt; """",False,True)</Hidden>
              </Visibility>
            </TablixMember>
            <TablixMember>
              <Visibility>
                <Hidden>=iif(fields!Party_PAN_NO.Value &lt;&gt; """" ,false,true)</Hidden>
              </Visibility>
            </TablixMember>
          </TablixMembers>
        </TablixRowHierarchy>
        <DataSetName>DsMain</DataSetName>
        <Top>0.64723in</Top>
        <Left>5.67751in</Left>
        <Height>1.62083in</Height>
        <Width>2.97297in</Width>
        <ZIndex>5</ZIndex>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
        </Style>
      </Tablix>
      <Tablix Name=""Tablix7"">
        <TablixBody>
          <TablixColumns>
            <TablixColumn>
              <Width>0.71163in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.22526in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>1.42069in</Width>
            </TablixColumn>
          </TablixColumns>
          <TablixRows>
            <TablixRow>
              <Height>0.19132in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox52"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!DocIdCaption.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox34</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox54"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox52</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""DocNo4"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!DocNo.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>DocNo</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.21216in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox73"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!DocIdCaptionDate.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox31</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox77"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox49</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""DocDate3"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!DocDate.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <Format>dd/MMM/yy</Format>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>DocDate</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox200"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Reverse Charge</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox194</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox201"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox195</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                          <Style>None</Style>
                          <Width>1pt</Width>
                        </TopBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""ReverseCharge3"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!ReverseCharge.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>ReverseCharge</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox97"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Process</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox5</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox98"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox9</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox99"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!ProcessName.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox10</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox114"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Interaction.iif(Max(Fields!FromGodownName.Value) &lt;&gt; """", ""To Godown"", ""Godown"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox16</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox116"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox17</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox120"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!GodownName.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox20</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox31"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>From Godown</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox11</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox94"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox14</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox95"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!FromGodownName.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox15</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
          </TablixRows>
        </TablixBody>
        <TablixColumnHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember />
            <TablixMember />
          </TablixMembers>
        </TablixColumnHierarchy>
        <TablixRowHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember>
              <Visibility>
                <Hidden>=iif(max(fields!FromGodownName.Value) &lt;&gt; """",False,True)</Hidden>
              </Visibility>
            </TablixMember>
          </TablixMembers>
        </TablixRowHierarchy>
        <DataSetName>DsMain</DataSetName>
        <Top>0.64341in</Top>
        <Left>8.71992in</Left>
        <Height>1.40348in</Height>
        <Width>2.35758in</Width>
        <ZIndex>6</ZIndex>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
        </Style>
      </Tablix>
      <Tablix Name=""Tablix6"">
        <TablixBody>
          <TablixColumns>
            <TablixColumn>
              <Width>0.45778in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>1.82947in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.85417in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.74894in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.34181in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>1.19585in</Width>
            </TablixColumn>
          </TablixColumns>
          <TablixRows>
            <TablixRow>
              <Height>0.22111in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox323"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Sr.</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox320</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox261"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Max(Fields!ProductCaption.Value)</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox251</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox103"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Max(Fields!SalesTaxProductCodeCaption.Value)</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox68</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox12"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Qty</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox10</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox363"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Unit</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox362</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Color>Black</Color>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox260"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Remarks</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox259</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox332"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=RowNumber(Nothing)</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox321</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""ProductName2"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Code.PrintProduct(Fields!ProductName.Value,Fields!ProductGroupCaption.Value,fields!ProductGroupName.Value,Fields!SpecificationCaption.Value,fields!Specification.Value,fields!Dimension1Caption.Value,fields!Dimension1Name.Value,fields!Dimension2Caption.Value,fields!Dimension2Name.Value,fields!Dimension3Caption.Value,fields!Dimension3Name.Value,fields!Dimension4Caption.Value,fields!Dimension4Name.Value,Fields!ProductUidCaption.Value,fields!ProductUidName.Value)</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>ProductName</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox104"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!SalesTaxProductCodes.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox69</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Qty6"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Interaction.iif(Fields!Qty.Value &lt;&gt; 0, Microsoft.VisualBasic.Strings.formatnumber(Fields!Qty.Value, Fields!DecimalPlaces.Value), ""-"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Qty</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""UnitName6"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!UnitName.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>UnitName</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Remark7"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Code.PrintLineRemark(Fields!LineRemark.Value,Fields!LotNo.Value,"""","""")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Remark</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox46"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>TOTAL</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox42</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox331"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox331</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox60"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Interaction.iif(Sum(Fields!Qty.Value) &lt;&gt; 0, Microsoft.VisualBasic.Strings.formatnumber(Sum(Fields!Qty.Value), Max(Fields!DecimalPlaces.Value)), ""-"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox48</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox30"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox29</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox55"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox54</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
          </TablixRows>
        </TablixBody>
        <TablixColumnHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
          </TablixMembers>
        </TablixColumnHierarchy>
        <TablixRowHierarchy>
          <TablixMembers>
            <TablixMember>
              <KeepWithGroup>After</KeepWithGroup>
              <RepeatOnNewPage>true</RepeatOnNewPage>
            </TablixMember>
            <TablixMember>
              <Group Name=""StockHeaderId2"">
                <GroupExpressions>
                  <GroupExpression>=Fields!StockHeaderId.Value</GroupExpression>
                </GroupExpressions>
              </Group>
              <SortExpressions>
                <SortExpression>
                  <Value>=Fields!StockHeaderId.Value</Value>
                </SortExpression>
              </SortExpressions>
              <TablixMembers>
                <TablixMember>
                  <Group Name=""Details2"" />
                  <SortExpressions>
                    <SortExpression>
                      <Value>=Fields!StockHeaderId.Value</Value>
                    </SortExpression>
                  </SortExpressions>
                  <TablixMembers>
                    <TablixMember />
                  </TablixMembers>
                </TablixMember>
                <TablixMember>
                  <KeepWithGroup>Before</KeepWithGroup>
                </TablixMember>
              </TablixMembers>
            </TablixMember>
          </TablixMembers>
        </TablixRowHierarchy>
        <DataSetName>DsMain</DataSetName>
        <Top>2.325in</Top>
        <Left>5.66034in</Left>
        <Height>0.72111in</Height>
        <Width>5.42802in</Width>
        <ZIndex>7</ZIndex>
        <Visibility>
          <Hidden>=Microsoft.VisualBasic.Interaction.IIF(Max(Fields!CostCenterCnt.Value) &gt; 0, True, False)</Hidden>
        </Visibility>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
        </Style>
      </Tablix>
      <Tablix Name=""Tablix9"">
        <TablixBody>
          <TablixColumns>
            <TablixColumn>
              <Width>1.17757in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>1.3973in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>1.3528in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>1.47594in</Width>
            </TablixColumn>
          </TablixColumns>
          <TablixRows>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox83"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox83</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox84"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox84</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox85"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox85</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox86"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox86</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.22292in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox9"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Received By</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox6</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox5"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Checked By</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox1</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox57"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Passed By</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox40</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
          </TablixRows>
        </TablixBody>
        <TablixColumnHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
          </TablixMembers>
        </TablixColumnHierarchy>
        <TablixRowHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember />
          </TablixMembers>
        </TablixRowHierarchy>
        <DataSetName>DsMain</DataSetName>
        <Top>4.08792in</Top>
        <Left>5.68475in</Left>
        <Height>0.47292in</Height>
        <Width>5.40361in</Width>
        <ZIndex>8</ZIndex>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
        </Style>
      </Tablix>
      <Tablix Name=""Tablix11"">
        <TablixBody>
          <TablixColumns>
            <TablixColumn>
              <Width>0.84334in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.21744in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>1.91219in</Width>
            </TablixColumn>
          </TablixColumns>
          <TablixRows>
            <TablixRow>
              <Height>0.2in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox27"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!PartyCaption.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox26</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                          <Width>1pt</Width>
                        </TopBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""BuyerName"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Format>0.0000;(0.0000);'-'</Format>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>BuyerName</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                          <Width>1pt</Width>
                        </TopBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Jobworker"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!PartyName.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Jobworker</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                          <Width>1pt</Width>
                        </TopBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.21042in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox45"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox3</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                          <Width>1pt</Width>
                        </TopBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox118"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox117</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                          <Width>1pt</Width>
                        </TopBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox134"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!PartySuffix.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox134</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                          <Width>1pt</Width>
                        </TopBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.22083in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox196"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Address</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox177</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox76"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox60</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Address"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!PartyAddress.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Address</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.23958in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox252"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Mobile No.</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox252</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox81"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox60</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""MobileNo"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!PartyMobileNo.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>MobileNo</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox149"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>GSTIN</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox146</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox143"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox143</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox150"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!Party_GST_NO.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox145</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox115"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value EvaluationMode=""Constant"">Aadhar No</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox115</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox93"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox60</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""TinNo"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!Party_AADHAR_NO.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>TinNo</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox147"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value EvaluationMode=""Constant"">PAN No</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox147</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox148"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox148</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""AADHAR"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!Party_PAN_NO.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>AADHAR</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
          </TablixRows>
        </TablixBody>
        <TablixColumnHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember />
            <TablixMember />
          </TablixMembers>
        </TablixColumnHierarchy>
        <TablixRowHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember>
              <Visibility>
                <Hidden>=iif(max(fields!Party_GST_NO.Value) &lt;&gt; """",False,True)</Hidden>
              </Visibility>
            </TablixMember>
            <TablixMember>
              <Visibility>
                <Hidden>=iif(max(fields!Party_AADHAR_NO.Value) &lt;&gt; """",False,True)</Hidden>
              </Visibility>
            </TablixMember>
            <TablixMember>
              <Visibility>
                <Hidden>=iif(fields!Party_PAN_NO.Value &lt;&gt; """" ,false,true)</Hidden>
              </Visibility>
            </TablixMember>
          </TablixMembers>
        </TablixRowHierarchy>
        <DataSetName>DsMain</DataSetName>
        <Top>0.65488in</Top>
        <Left>0.06627in</Left>
        <Height>1.62083in</Height>
        <Width>2.97297in</Width>
        <ZIndex>9</ZIndex>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
        </Style>
      </Tablix>
      <Tablix Name=""Tablix12"">
        <TablixBody>
          <TablixColumns>
            <TablixColumn>
              <Width>0.82584in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.22526in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>1.42069in</Width>
            </TablixColumn>
          </TablixColumns>
          <TablixRows>
            <TablixRow>
              <Height>0.19132in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox47"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!DocIdCaption.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox34</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox63"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox52</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""DocNo"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!DocNo.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>DocNo</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.21216in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox49"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!DocIdCaptionDate.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox31</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox64"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox49</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""DocDate"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!DocDate.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <Format>dd/MMM/yy</Format>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>DocDate</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox194"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Reverse Charge</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox194</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox195"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox195</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                          <Style>None</Style>
                          <Width>1pt</Width>
                        </TopBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""ReverseCharge"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!ReverseCharge.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>ReverseCharge</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox70"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Process</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox5</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox71"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox9</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox72"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!ProcessName.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox10</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox122"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Interaction.iif(Max(Fields!FromGodownName.Value) &lt;&gt; """", ""To Godown"", ""Godown"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox16</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox123"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox17</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox124"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!GodownName.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox20</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox32"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>From Godown</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox11</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox68"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox14</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox69"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!FromGodownName.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox15</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
          </TablixRows>
        </TablixBody>
        <TablixColumnHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember />
            <TablixMember />
          </TablixMembers>
        </TablixColumnHierarchy>
        <TablixRowHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember>
              <Visibility>
                <Hidden>=iif(max(fields!FromGodownName.Value) &lt;&gt; """",False,True)</Hidden>
              </Visibility>
            </TablixMember>
          </TablixMembers>
        </TablixRowHierarchy>
        <DataSetName>DsMain</DataSetName>
        <Top>0.65591in</Top>
        <Left>3.10869in</Left>
        <Height>1.40348in</Height>
        <Width>2.47179in</Width>
        <ZIndex>10</ZIndex>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
        </Style>
      </Tablix>
      <Tablix Name=""Tablix13"">
        <TablixBody>
          <TablixColumns>
            <TablixColumn>
              <Width>0.43198in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>1.47708in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>1in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.45602in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.74094in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>1.30648in</Width>
            </TablixColumn>
          </TablixColumns>
          <TablixRows>
            <TablixRow>
              <Height>0.23013in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox325"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Sr.</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox320</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox258"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Max(Fields!ProductCaption.Value)</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox251</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox74"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Max(Fields!SalesTaxProductCodeCaption.Value)</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox68</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox17"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Qty</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox10</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox365"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Unit</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox362</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Color>Black</Color>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox262"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Remarks</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox259</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox327"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=RowNumber(Nothing)</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox321</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""ProductName3"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Code.PrintProduct(Fields!ProductName.Value,Fields!ProductGroupCaption.Value,fields!ProductGroupName.Value,Fields!SpecificationCaption.Value,fields!Specification.Value,fields!Dimension1Caption.Value,fields!Dimension1Name.Value,fields!Dimension2Caption.Value,fields!Dimension2Name.Value,fields!Dimension3Caption.Value,fields!Dimension3Name.Value,fields!Dimension4Caption.Value,fields!Dimension4Name.Value,Fields!ProductUidCaption.Value,fields!ProductUidName.Value)</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>ProductName</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox75"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!SalesTaxProductCodes.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox69</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Qty5"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Interaction.iif(Fields!Qty.Value &lt;&gt; 0, Microsoft.VisualBasic.Strings.formatnumber(Fields!Qty.Value, Fields!DecimalPlaces.Value), ""-"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Qty</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""UnitName4"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!UnitName.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>UnitName</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Remark5"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Code.PrintLineRemark(Fields!LineRemark.Value,Fields!LotNo.Value,"""","""")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Remark</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.22083in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox48"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>TOTAL</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox42</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox271"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox271</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox58"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Interaction.iif(Sum(Fields!Qty.Value) &lt;&gt; 0, Microsoft.VisualBasic.Strings.formatnumber(Sum(Fields!Qty.Value), Max(Fields!DecimalPlaces.Value)), ""-"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox48</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox59"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox54</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox244"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox244</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
          </TablixRows>
        </TablixBody>
        <TablixColumnHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
          </TablixMembers>
        </TablixColumnHierarchy>
        <TablixRowHierarchy>
          <TablixMembers>
            <TablixMember>
              <KeepWithGroup>After</KeepWithGroup>
              <RepeatOnNewPage>true</RepeatOnNewPage>
            </TablixMember>
            <TablixMember>
              <Group Name=""StockHeaderId3"">
                <GroupExpressions>
                  <GroupExpression>=Fields!StockHeaderId.Value</GroupExpression>
                </GroupExpressions>
              </Group>
              <SortExpressions>
                <SortExpression>
                  <Value>=Fields!StockHeaderId.Value</Value>
                </SortExpression>
              </SortExpressions>
              <TablixMembers>
                <TablixMember>
                  <Group Name=""Details3"" />
                  <SortExpressions>
                    <SortExpression>
                      <Value>=Fields!StockHeaderId.Value</Value>
                    </SortExpression>
                  </SortExpressions>
                  <TablixMembers>
                    <TablixMember />
                  </TablixMembers>
                </TablixMember>
                <TablixMember>
                  <KeepWithGroup>Before</KeepWithGroup>
                </TablixMember>
              </TablixMembers>
            </TablixMember>
          </TablixMembers>
        </TablixRowHierarchy>
        <DataSetName>DsMain</DataSetName>
        <Top>2.32015in</Top>
        <Left>0.05377in</Left>
        <Height>0.70096in</Height>
        <Width>5.4125in</Width>
        <ZIndex>11</ZIndex>
        <Visibility>
          <Hidden>=Microsoft.VisualBasic.Interaction.IIF(Max(Fields!CostCenterCnt.Value) &gt; 0, True, False)</Hidden>
        </Visibility>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
        </Style>
      </Tablix>
      <Tablix Name=""Tablix15"">
        <TablixBody>
          <TablixColumns>
            <TablixColumn>
              <Width>1.17757in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>1.3973in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>1.3528in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>1.46509in</Width>
            </TablixColumn>
          </TablixColumns>
          <TablixRows>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox88"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox83</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox89"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox84</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox90"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox85</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox91"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox86</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.22292in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox14"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Received By</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox6</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox7"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Checked By</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox1</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox61"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Passed By</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox40</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
          </TablixRows>
        </TablixBody>
        <TablixColumnHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
          </TablixMembers>
        </TablixColumnHierarchy>
        <TablixRowHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember />
          </TablixMembers>
        </TablixRowHierarchy>
        <DataSetName>DsMain</DataSetName>
        <Top>4.08153in</Top>
        <Left>0.07351in</Left>
        <Height>0.47292in</Height>
        <Width>5.39276in</Width>
        <ZIndex>12</ZIndex>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
        </Style>
      </Tablix>
      <Tablix Name=""Tablix18"">
        <TablixBody>
          <TablixColumns>
            <TablixColumn>
              <Width>0.40286in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>1.20721in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.71844in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.69927in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.48958in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.58865in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>1.30648in</Width>
            </TablixColumn>
          </TablixColumns>
          <TablixRows>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox328"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Sr.</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox320</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox259"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Max(Fields!ProductCaption.Value)</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox251</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox92"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Max(Fields!SalesTaxProductCodeCaption.Value)</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox71</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox53"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Max(Fields!CostCenterCaption.Value)</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox46</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox23"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Qty</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox10</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox366"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Unit</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox362</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Color>Black</Color>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox263"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Remarks</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox259</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox329"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=RowNumber(Nothing)</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox321</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""ProductName4"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Code.PrintProduct(Fields!ProductName.Value,Fields!ProductGroupCaption.Value,fields!ProductGroupName.Value,Fields!SpecificationCaption.Value,fields!Specification.Value,fields!Dimension1Caption.Value,fields!Dimension1Name.Value,fields!Dimension2Caption.Value,fields!Dimension2Name.Value,fields!Dimension3Caption.Value,fields!Dimension3Name.Value,fields!Dimension4Caption.Value,fields!Dimension4Name.Value,Fields!ProductUidCaption.Value,fields!ProductUidName.Value)</Value>
                              <MarkupType>HTML</MarkupType>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>ProductName</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox100"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!SalesTaxProductCodes.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox72</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""CostCenterName"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!CostCenterName.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>CostCenterName</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Qty4"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Interaction.iif(Fields!Qty.Value &lt;&gt; 0, Microsoft.VisualBasic.Strings.formatnumber(Fields!Qty.Value, Fields!DecimalPlaces.Value), ""-"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Qty</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""UnitName5"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!UnitName.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>UnitName</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Remark6"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Code.PrintLineRemark(Fields!LineRemark.Value,Fields!LotNo.Value,"""","""")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Remark</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox51"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>TOTAL</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox42</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox101"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox73</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox102"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox73</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox62"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Interaction.iif(Sum(Fields!Qty.Value) &lt;&gt; 0, Microsoft.VisualBasic.Strings.formatnumber(Sum(Fields!Qty.Value), Max(Fields!DecimalPlaces.Value)), ""-"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox48</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox34"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox29</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox35"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox29</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
          </TablixRows>
        </TablixBody>
        <TablixColumnHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
          </TablixMembers>
        </TablixColumnHierarchy>
        <TablixRowHierarchy>
          <TablixMembers>
            <TablixMember>
              <KeepWithGroup>After</KeepWithGroup>
            </TablixMember>
            <TablixMember>
              <Group Name=""Details"" />
            </TablixMember>
            <TablixMember>
              <KeepWithGroup>Before</KeepWithGroup>
            </TablixMember>
          </TablixMembers>
        </TablixRowHierarchy>
        <DataSetName>DsMain</DataSetName>
        <Top>3.09055in</Top>
        <Left>0.05377in</Left>
        <Height>0.75in</Height>
        <Width>5.4125in</Width>
        <ZIndex>13</ZIndex>
        <Visibility>
          <Hidden>=Microsoft.VisualBasic.Interaction.IIF(Max(Fields!CostCenterCnt.Value) &gt; 0, False, True)</Hidden>
        </Visibility>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
        </Style>
      </Tablix>
      <Tablix Name=""Tablix19"">
        <TablixBody>
          <TablixColumns>
            <TablixColumn>
              <Width>0.51042in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>1.06742in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.70902in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.77083in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.76485in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.41223in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>1.19324in</Width>
            </TablixColumn>
          </TablixColumns>
          <TablixRows>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox330"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Sr.</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox320</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox264"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Max(Fields!ProductCaption.Value)</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox251</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox105"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Max(Fields!SalesTaxProductCodeCaption.Value)</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox71</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox108"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Max(Fields!CostCenterCaption.Value)</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox46</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox24"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Qty</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox10</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox367"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Unit</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox362</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Color>Black</Color>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox265"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Remarks</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox259</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox333"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=RowNumber(Nothing)</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox321</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox342"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Code.PrintProduct(Fields!ProductName.Value,Fields!ProductGroupCaption.Value,fields!ProductGroupName.Value,Fields!SpecificationCaption.Value,fields!Specification.Value,fields!Dimension1Caption.Value,fields!Dimension1Name.Value,fields!Dimension2Caption.Value,fields!Dimension2Name.Value,fields!Dimension3Caption.Value,fields!Dimension3Name.Value,fields!Dimension4Caption.Value,fields!Dimension4Name.Value,Fields!ProductUidCaption.Value,fields!ProductUidName.Value)</Value>
                              <Style>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox342</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox106"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!SalesTaxProductCodes.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox72</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""CostCenterName2"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!CostCenterName.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>CostCenterName</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Qty7"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Interaction.iif(Fields!Qty.Value &lt;&gt; 0, Microsoft.VisualBasic.Strings.formatnumber(Fields!Qty.Value, Fields!DecimalPlaces.Value), ""-"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Qty</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""UnitName7"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!UnitName.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>UnitName</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Remark8"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Code.PrintLineRemark(Fields!LineRemark.Value,Fields!LotNo.Value,"""","""")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Remark</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox56"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>TOTAL</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox42</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox107"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox73</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox109"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox73</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox110"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Interaction.iif(Sum(Fields!Qty.Value) &lt;&gt; 0, Microsoft.VisualBasic.Strings.formatnumber(Sum(Fields!Qty.Value), Max(Fields!DecimalPlaces.Value)), ""-"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox48</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox36"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox29</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox37"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox29</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
          </TablixRows>
        </TablixBody>
        <TablixColumnHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
          </TablixMembers>
        </TablixColumnHierarchy>
        <TablixRowHierarchy>
          <TablixMembers>
            <TablixMember>
              <KeepWithGroup>After</KeepWithGroup>
            </TablixMember>
            <TablixMember>
              <Group Name=""Details1"" />
            </TablixMember>
            <TablixMember>
              <KeepWithGroup>Before</KeepWithGroup>
            </TablixMember>
          </TablixMembers>
        </TablixRowHierarchy>
        <DataSetName>DsMain</DataSetName>
        <Top>3.09055in</Top>
        <Left>5.66034in</Left>
        <Height>0.75in</Height>
        <Width>5.42802in</Width>
        <ZIndex>14</ZIndex>
        <Visibility>
          <Hidden>=Microsoft.VisualBasic.Interaction.IIF(Max(Fields!CostCenterCnt.Value) &gt; 0, False, True)</Hidden>
        </Visibility>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
        </Style>
      </Tablix>
      <Tablix Name=""Tablix4"">
        <TablixBody>
          <TablixColumns>
            <TablixColumn>
              <Width>0.61043in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.20634in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>1.67131in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.91258in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.14459in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.99811in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.81685in</Width>
            </TablixColumn>
          </TablixColumns>
          <TablixRows>
            <TablixRow>
              <Height>0.25986in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox387"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>14pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox386</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Dotted</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <VerticalAlign>Middle</VerticalAlign>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>7</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell />
                <TablixCell />
                <TablixCell />
                <TablixCell />
                <TablixCell />
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.32292in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""ReportTitle6"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!ReportTitle.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>14pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>ReportTitle</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <VerticalAlign>Middle</VerticalAlign>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>7</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell />
                <TablixCell />
                <TablixCell />
                <TablixCell />
                <TablixCell />
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox42"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Name</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox9</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox43"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox10</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""DocNo5"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!PersonName.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>DocNo</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox44"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Gate Pass No.</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox23</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox65"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox11</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""DocNo7"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!DocNo.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>DocNo</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox66"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Godown</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox9</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox67"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox10</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""DocNo9"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!GodownName.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>DocNo</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox79"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Date</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox24</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox82"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox19</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""DocDate5"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!DocDate.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <Format>dd/MMM/yy</Format>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>DocDate</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox272"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox271</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox217"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Remark</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox214</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox96"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox10</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""DocNo10"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!Remark.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>DocNo</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox111"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Reference No.</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox24</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox112"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox19</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""DocDate6"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!ReferenceDocNo.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>DocDate</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox222"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Sr</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox219</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox172"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Product</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox168</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox173"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Specification</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox170</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox180"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Qty</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox172</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox181"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Unit</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox174</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox223"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=RowNumber(Nothing)</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox220</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Product3"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!Product.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Product</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Specification2"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!Specification.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Specification</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Qty8"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Strings.FormatNumber(Fields!Qty.Value, Fields!DecimalPlaces.Value)</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Qty</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""UnitName8"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!UnitName.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>UnitName</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox218"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Total :</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox215</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox113"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox102</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox189"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox187</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox224"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox216</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox234"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Strings.FormatNumber(Sum(Fields!Qty.Value), Max(Fields!DecimalPlaces.Value))</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox217</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox277"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox275</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.36458in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox121"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox1</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox125"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox2</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox190"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox188</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox126"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox3</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox127"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox4</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox128"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox5</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox278"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox276</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox129"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!SignatoryleftCaption.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox6</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>3</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell />
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox130"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!SignatoryMiddleCaption.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox8</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox131"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!SignatoryRightCaption.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox15</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
              </TablixCells>
            </TablixRow>
          </TablixRows>
        </TablixBody>
        <TablixColumnHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
          </TablixMembers>
        </TablixColumnHierarchy>
        <TablixRowHierarchy>
          <TablixMembers>
            <TablixMember>
              <KeepWithGroup>After</KeepWithGroup>
            </TablixMember>
            <TablixMember>
              <KeepWithGroup>After</KeepWithGroup>
            </TablixMember>
            <TablixMember>
              <KeepWithGroup>After</KeepWithGroup>
            </TablixMember>
            <TablixMember>
              <KeepWithGroup>After</KeepWithGroup>
            </TablixMember>
            <TablixMember>
              <KeepWithGroup>After</KeepWithGroup>
            </TablixMember>
            <TablixMember>
              <KeepWithGroup>After</KeepWithGroup>
            </TablixMember>
            <TablixMember>
              <Group Name=""Details6"" />
            </TablixMember>
            <TablixMember>
              <KeepWithGroup>Before</KeepWithGroup>
            </TablixMember>
            <TablixMember>
              <KeepWithGroup>Before</KeepWithGroup>
            </TablixMember>
            <TablixMember>
              <KeepWithGroup>Before</KeepWithGroup>
            </TablixMember>
          </TablixMembers>
        </TablixRowHierarchy>
        <DataSetName>DsGatePass</DataSetName>
        <Top>4.62528in</Top>
        <Left>5.6548in</Left>
        <Height>2.69736in</Height>
        <Width>5.36021in</Width>
        <ZIndex>15</ZIndex>
        <Visibility>
          <Hidden>=Microsoft.VisualBasic.Interaction.iif(First(Fields!GatePassHeaderId.Value, ""DsMain"") = 0, True, False)</Hidden>
        </Visibility>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
        </Style>
      </Tablix>
    </ReportItems>
    <Height>7.32265in</Height>
    <Style />
  </Body>
  <Width>11.08836in</Width>
  <Page>
    <PageHeader>
      <Height>0.6032in</Height>
      <PrintOnFirstPage>true</PrintOnFirstPage>
      <PrintOnLastPage>true</PrintOnLastPage>
      <ReportItems>
        <Textbox Name=""ReportTitle2"">
          <CanGrow>true</CanGrow>
          <KeepTogether>true</KeepTogether>
          <Paragraphs>
            <Paragraph>
              <TextRuns>
                <TextRun>
                  <Value>=First(Fields!ReportTitle.Value, ""DsMain"")</Value>
                  <Style>
                    <FontFamily>Tahoma</FontFamily>
                    <FontSize>14pt</FontSize>
                    <FontWeight>Bold</FontWeight>
                  </Style>
                </TextRun>
              </TextRuns>
              <Style>
                <TextAlign>Center</TextAlign>
              </Style>
            </Paragraph>
          </Paragraphs>
          <rd:DefaultName>ReportTitle</rd:DefaultName>
          <Top>0.03028in</Top>
          <Left>0.06626in</Left>
          <Height>0.25in</Height>
          <Width>5.40001in</Width>
          <Visibility>
            <Hidden>=iif(Globals!PageNumber=1,True,False)</Hidden>
          </Visibility>
          <Style>
            <Border>
              <Style>None</Style>
            </Border>
            <TopBorder>
              <Color>Black</Color>
              <Style>Solid</Style>
              <Width>1pt</Width>
            </TopBorder>
            <BottomBorder>
              <Color>Black</Color>
              <Style>Solid</Style>
              <Width>1pt</Width>
            </BottomBorder>
            <PaddingLeft>2pt</PaddingLeft>
            <PaddingRight>2pt</PaddingRight>
            <PaddingTop>2pt</PaddingTop>
            <PaddingBottom>2pt</PaddingBottom>
          </Style>
        </Textbox>
        <Textbox Name=""Textbox4"">
          <CanGrow>true</CanGrow>
          <KeepTogether>true</KeepTogether>
          <Paragraphs>
            <Paragraph>
              <TextRuns>
                <TextRun>
                  <Value>=""Name : "" + First(Fields!PartyName.Value, ""DsMain"")</Value>
                  <Style>
                    <FontFamily>Tahoma</FontFamily>
                    <FontSize>9pt</FontSize>
                    <FontWeight>Bold</FontWeight>
                  </Style>
                </TextRun>
              </TextRuns>
              <Style />
            </Paragraph>
          </Paragraphs>
          <rd:DefaultName>Textbox4</rd:DefaultName>
          <Top>0.30806in</Top>
          <Left>0.06626in</Left>
          <Height>0.25in</Height>
          <Width>2.97298in</Width>
          <ZIndex>1</ZIndex>
          <Visibility>
            <Hidden>=iif(Globals!PageNumber=1,True,False)</Hidden>
          </Visibility>
          <Style>
            <Border>
              <Style>None</Style>
            </Border>
            <BottomBorder>
              <Color>Black</Color>
              <Style>Solid</Style>
              <Width>1pt</Width>
            </BottomBorder>
            <PaddingLeft>2pt</PaddingLeft>
            <PaddingRight>2pt</PaddingRight>
            <PaddingTop>2pt</PaddingTop>
            <PaddingBottom>2pt</PaddingBottom>
          </Style>
        </Textbox>
        <Textbox Name=""Textbox8"">
          <CanGrow>true</CanGrow>
          <KeepTogether>true</KeepTogether>
          <Paragraphs>
            <Paragraph>
              <TextRuns>
                <TextRun>
                  <Value>=""Slip No : "" + First(Fields!DocNo.Value, ""DsMain"")</Value>
                  <Style>
                    <FontFamily>Tahoma</FontFamily>
                    <FontSize>9pt</FontSize>
                    <FontWeight>Bold</FontWeight>
                  </Style>
                </TextRun>
              </TextRuns>
              <Style />
            </Paragraph>
          </Paragraphs>
          <rd:DefaultName>Textbox4</rd:DefaultName>
          <Top>0.30806in</Top>
          <Left>3.10869in</Left>
          <Height>0.25in</Height>
          <Width>2.35758in</Width>
          <ZIndex>2</ZIndex>
          <Visibility>
            <Hidden>=iif(Globals!PageNumber=1,True,False)</Hidden>
          </Visibility>
          <Style>
            <Border>
              <Style>None</Style>
            </Border>
            <BottomBorder>
              <Color>Black</Color>
              <Style>Solid</Style>
              <Width>1pt</Width>
            </BottomBorder>
            <PaddingLeft>2pt</PaddingLeft>
            <PaddingRight>2pt</PaddingRight>
            <PaddingTop>2pt</PaddingTop>
            <PaddingBottom>2pt</PaddingBottom>
          </Style>
        </Textbox>
        <Textbox Name=""ReportTitle5"">
          <CanGrow>true</CanGrow>
          <KeepTogether>true</KeepTogether>
          <Paragraphs>
            <Paragraph>
              <TextRuns>
                <TextRun>
                  <Value>=First(Fields!ReportTitle.Value, ""DsMain"")</Value>
                  <Style>
                    <FontFamily>Tahoma</FontFamily>
                    <FontSize>14pt</FontSize>
                    <FontWeight>Bold</FontWeight>
                  </Style>
                </TextRun>
              </TextRuns>
              <Style>
                <TextAlign>Center</TextAlign>
              </Style>
            </Paragraph>
          </Paragraphs>
          <rd:DefaultName>ReportTitle</rd:DefaultName>
          <Top>0.02194in</Top>
          <Left>5.66034in</Left>
          <Height>0.25in</Height>
          <Width>5.41717in</Width>
          <ZIndex>3</ZIndex>
          <Visibility>
            <Hidden>=iif(Globals!PageNumber=1,True,False)</Hidden>
          </Visibility>
          <Style>
            <Border>
              <Style>None</Style>
            </Border>
            <TopBorder>
              <Color>Black</Color>
              <Style>Solid</Style>
              <Width>1pt</Width>
            </TopBorder>
            <BottomBorder>
              <Color>Black</Color>
              <Style>Solid</Style>
              <Width>1pt</Width>
            </BottomBorder>
            <PaddingLeft>2pt</PaddingLeft>
            <PaddingRight>2pt</PaddingRight>
            <PaddingTop>2pt</PaddingTop>
            <PaddingBottom>2pt</PaddingBottom>
          </Style>
        </Textbox>
        <Textbox Name=""Textbox15"">
          <CanGrow>true</CanGrow>
          <KeepTogether>true</KeepTogether>
          <Paragraphs>
            <Paragraph>
              <TextRuns>
                <TextRun>
                  <Value>=""Name : "" + First(Fields!PartyName.Value, ""DsMain"")</Value>
                  <Style>
                    <FontFamily>Tahoma</FontFamily>
                    <FontSize>9pt</FontSize>
                    <FontWeight>Bold</FontWeight>
                  </Style>
                </TextRun>
              </TextRuns>
              <Style />
            </Paragraph>
          </Paragraphs>
          <rd:DefaultName>Textbox4</rd:DefaultName>
          <Top>0.29972in</Top>
          <Left>5.66034in</Left>
          <Height>0.25in</Height>
          <Width>2.99014in</Width>
          <ZIndex>4</ZIndex>
          <Visibility>
            <Hidden>=iif(Globals!PageNumber=1,True,False)</Hidden>
          </Visibility>
          <Style>
            <Border>
              <Style>None</Style>
            </Border>
            <BottomBorder>
              <Color>Black</Color>
              <Style>Solid</Style>
              <Width>1pt</Width>
            </BottomBorder>
            <PaddingLeft>2pt</PaddingLeft>
            <PaddingRight>2pt</PaddingRight>
            <PaddingTop>2pt</PaddingTop>
            <PaddingBottom>2pt</PaddingBottom>
          </Style>
        </Textbox>
        <Textbox Name=""Textbox16"">
          <CanGrow>true</CanGrow>
          <KeepTogether>true</KeepTogether>
          <Paragraphs>
            <Paragraph>
              <TextRuns>
                <TextRun>
                  <Value>=""Slip No : "" + First(Fields!DocNo.Value, ""DsMain"")</Value>
                  <Style>
                    <FontFamily>Tahoma</FontFamily>
                    <FontSize>9pt</FontSize>
                    <FontWeight>Bold</FontWeight>
                  </Style>
                </TextRun>
              </TextRuns>
              <Style />
            </Paragraph>
          </Paragraphs>
          <rd:DefaultName>Textbox4</rd:DefaultName>
          <Top>0.29972in</Top>
          <Left>8.70277in</Left>
          <Height>0.25in</Height>
          <Width>2.37474in</Width>
          <ZIndex>5</ZIndex>
          <Visibility>
            <Hidden>=iif(Globals!PageNumber=1,True,False)</Hidden>
          </Visibility>
          <Style>
            <Border>
              <Style>None</Style>
            </Border>
            <BottomBorder>
              <Color>Black</Color>
              <Style>Solid</Style>
              <Width>1pt</Width>
            </BottomBorder>
            <PaddingLeft>2pt</PaddingLeft>
            <PaddingRight>2pt</PaddingRight>
            <PaddingTop>2pt</PaddingTop>
            <PaddingBottom>2pt</PaddingBottom>
          </Style>
        </Textbox>
      </ReportItems>
      <Style>
        <Border>
          <Style>None</Style>
        </Border>
      </Style>
    </PageHeader>
    <PageFooter>
      <Height>0.33472in</Height>
      <PrintOnFirstPage>true</PrintOnFirstPage>
      <PrintOnLastPage>true</PrintOnLastPage>
      <ReportItems>
        <Textbox Name=""Textbox19"">
          <CanGrow>true</CanGrow>
          <KeepTogether>true</KeepTogether>
          <Paragraphs>
            <Paragraph>
              <TextRuns>
                <TextRun>
                  <Value>Print Date : </Value>
                  <Style>
                    <FontFamily>tahoma</FontFamily>
                    <FontSize>8pt</FontSize>
                    <Format>dd-MMM-yy hh:mm:ss tt</Format>
                  </Style>
                </TextRun>
                <TextRun>
                  <Value>=Globals!ExecutionTime</Value>
                  <Style>
                    <FontFamily>tahoma</FontFamily>
                    <FontSize>8pt</FontSize>
                    <Format>dd-MMM-yy hh:mm:ss tt</Format>
                  </Style>
                </TextRun>
              </TextRuns>
              <Style>
                <TextAlign>Left</TextAlign>
              </Style>
            </Paragraph>
          </Paragraphs>
          <rd:DefaultName>Textbox19</rd:DefaultName>
          <Top>0.04167in</Top>
          <Left>0.07276in</Left>
          <Height>0.25in</Height>
          <Width>2.21944in</Width>
          <Style>
            <Border>
              <Style>None</Style>
            </Border>
            <PaddingLeft>2pt</PaddingLeft>
            <PaddingRight>2pt</PaddingRight>
            <PaddingTop>2pt</PaddingTop>
            <PaddingBottom>2pt</PaddingBottom>
          </Style>
        </Textbox>
        <Textbox Name=""Textbox20"">
          <CanGrow>true</CanGrow>
          <KeepTogether>true</KeepTogether>
          <Paragraphs>
            <Paragraph>
              <TextRuns>
                <TextRun>
                  <Value>Page </Value>
                  <Style>
                    <FontFamily>tahoma</FontFamily>
                    <FontSize>8pt</FontSize>
                  </Style>
                </TextRun>
                <TextRun>
                  <Value>=Globals!PageNumber</Value>
                  <Style>
                    <FontFamily>tahoma</FontFamily>
                    <FontSize>8pt</FontSize>
                  </Style>
                </TextRun>
                <TextRun>
                  <Value> of </Value>
                  <Style>
                    <FontFamily>tahoma</FontFamily>
                    <FontSize>8pt</FontSize>
                  </Style>
                </TextRun>
                <TextRun>
                  <Value>=Globals!TotalPages</Value>
                  <Style>
                    <FontFamily>tahoma</FontFamily>
                    <FontSize>8pt</FontSize>
                  </Style>
                </TextRun>
              </TextRuns>
              <Style>
                <TextAlign>Right</TextAlign>
              </Style>
            </Paragraph>
          </Paragraphs>
          <rd:DefaultName>Textbox18</rd:DefaultName>
          <Top>0.04206in</Top>
          <Left>3.10869in</Left>
          <Height>0.25in</Height>
          <Width>2.37593in</Width>
          <ZIndex>1</ZIndex>
          <Style>
            <Border>
              <Style>None</Style>
            </Border>
            <PaddingLeft>2pt</PaddingLeft>
            <PaddingRight>2pt</PaddingRight>
            <PaddingTop>2pt</PaddingTop>
            <PaddingBottom>2pt</PaddingBottom>
          </Style>
        </Textbox>
        <Textbox Name=""Textbox21"">
          <CanGrow>true</CanGrow>
          <KeepTogether>true</KeepTogether>
          <Paragraphs>
            <Paragraph>
              <TextRuns>
                <TextRun>
                  <Value>Print Date : </Value>
                  <Style>
                    <FontFamily>tahoma</FontFamily>
                    <FontSize>8pt</FontSize>
                    <Format>dd-MMM-yy hh:mm:ss tt</Format>
                  </Style>
                </TextRun>
                <TextRun>
                  <Value>=Globals!ExecutionTime</Value>
                  <Style>
                    <FontFamily>tahoma</FontFamily>
                    <FontSize>8pt</FontSize>
                    <Format>dd-MMM-yy hh:mm:ss tt</Format>
                  </Style>
                </TextRun>
              </TextRuns>
              <Style>
                <TextAlign>Left</TextAlign>
              </Style>
            </Paragraph>
          </Paragraphs>
          <rd:DefaultName>Textbox19</rd:DefaultName>
          <Top>0.04279in</Top>
          <Left>5.72815in</Left>
          <Height>0.23789in</Height>
          <Width>2.21944in</Width>
          <ZIndex>2</ZIndex>
          <Style>
            <Border>
              <Style>None</Style>
            </Border>
            <PaddingLeft>2pt</PaddingLeft>
            <PaddingRight>2pt</PaddingRight>
            <PaddingTop>2pt</PaddingTop>
            <PaddingBottom>2pt</PaddingBottom>
          </Style>
        </Textbox>
        <Textbox Name=""Textbox18"">
          <CanGrow>true</CanGrow>
          <KeepTogether>true</KeepTogether>
          <Paragraphs>
            <Paragraph>
              <TextRuns>
                <TextRun>
                  <Value>Page </Value>
                  <Style>
                    <FontFamily>tahoma</FontFamily>
                    <FontSize>8pt</FontSize>
                  </Style>
                </TextRun>
                <TextRun>
                  <Value>=Globals!PageNumber</Value>
                  <Style>
                    <FontFamily>tahoma</FontFamily>
                    <FontSize>8pt</FontSize>
                  </Style>
                </TextRun>
                <TextRun>
                  <Value> of </Value>
                  <Style>
                    <FontFamily>tahoma</FontFamily>
                    <FontSize>8pt</FontSize>
                  </Style>
                </TextRun>
                <TextRun>
                  <Value>=Globals!TotalPages</Value>
                  <Style>
                    <FontFamily>tahoma</FontFamily>
                    <FontSize>8pt</FontSize>
                  </Style>
                </TextRun>
              </TextRuns>
              <Style>
                <TextAlign>Right</TextAlign>
              </Style>
            </Paragraph>
          </Paragraphs>
          <rd:DefaultName>Textbox18</rd:DefaultName>
          <Top>0.03068in</Top>
          <Left>8.63908in</Left>
          <Height>0.25in</Height>
          <Width>2.37593in</Width>
          <ZIndex>3</ZIndex>
          <Style>
            <Border>
              <Style>None</Style>
            </Border>
            <PaddingLeft>2pt</PaddingLeft>
            <PaddingRight>2pt</PaddingRight>
            <PaddingTop>2pt</PaddingTop>
            <PaddingBottom>2pt</PaddingBottom>
          </Style>
        </Textbox>
      </ReportItems>
      <Style>
        <Border>
          <Style>None</Style>
        </Border>
      </Style>
    </PageFooter>
    <PageHeight>8.27in</PageHeight>
    <PageWidth>11.69in</PageWidth>
    <LeftMargin>0.1in</LeftMargin>
    <TopMargin>0.1in</TopMargin>
    <BottomMargin>0.1in</BottomMargin>
    <Style />
  </Page>
  <AutoRefresh>0</AutoRefresh>
  <DataSources>
    <DataSource Name=""Purchase"">
      <DataSourceReference>DSOCL</DataSourceReference>
      <rd:SecurityType>None</rd:SecurityType>
      <rd:DataSourceID>727bb65e-cc51-4d28-a097-a8ea7d087931</rd:DataSourceID>
    </DataSource>
  </DataSources>
  <DataSets>
    <DataSet Name=""DsMain"">
      <Query>
        <DataSourceName>Purchase</DataSourceName>
        <QueryParameters>
          <QueryParameter Name=""@Id"">
            <Value>=Parameters!Id.Value</Value>
          </QueryParameter>
        </QueryParameters>
        <CommandType>StoredProcedure</CommandType>
        <CommandText>Web.StdDocPrint_StockIssue</CommandText>
      </Query>
      <Fields>
        <Field Name=""StockHeaderId"">
          <DataField>StockHeaderId</DataField>
          <rd:TypeName>System.Int32</rd:TypeName>
        </Field>
        <Field Name=""DocTypeId"">
          <DataField>DocTypeId</DataField>
          <rd:TypeName>System.Int32</rd:TypeName>
        </Field>
        <Field Name=""DocNo"">
          <DataField>DocNo</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""DocIdCaption"">
          <DataField>DocIdCaption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""DocDate"">
          <DataField>DocDate</DataField>
          <rd:TypeName>System.DateTime</rd:TypeName>
        </Field>
        <Field Name=""GodownName"">
          <DataField>GodownName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""LotNo"">
          <DataField>LotNo</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""DocIdCaptionDate"">
          <DataField>DocIdCaptionDate</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""UnitName"">
          <DataField>UnitName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""ProcessName"">
          <DataField>ProcessName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Remark"">
          <DataField>Remark</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""DocumentTypeShortName"">
          <DataField>DocumentTypeShortName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""GatePassHeaderId"">
          <DataField>GatePassHeaderId</DataField>
          <rd:TypeName>System.Int32</rd:TypeName>
        </Field>
        <Field Name=""ModifiedBy"">
          <DataField>ModifiedBy</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""ProductName"">
          <DataField>ProductName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""ModifiedDate"">
          <DataField>ModifiedDate</DataField>
          <rd:TypeName>System.DateTime</rd:TypeName>
        </Field>
        <Field Name=""Qty"">
          <DataField>Qty</DataField>
          <rd:TypeName>System.Decimal</rd:TypeName>
        </Field>
        <Field Name=""Status"">
          <DataField>Status</DataField>
          <rd:TypeName>System.Int32</rd:TypeName>
        </Field>
        <Field Name=""ReverseCharge"">
          <DataField>ReverseCharge</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""SiteId"">
          <DataField>SiteId</DataField>
          <rd:TypeName>System.Int32</rd:TypeName>
        </Field>
        <Field Name=""CompanyName"">
          <DataField>CompanyName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""DivisionId"">
          <DataField>DivisionId</DataField>
          <rd:TypeName>System.Int32</rd:TypeName>
        </Field>
        <Field Name=""FromGodownName"">
          <DataField>FromGodownName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""PartyName"">
          <DataField>PartyName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""DecimalPlaces"">
          <DataField>DecimalPlaces</DataField>
          <rd:TypeName>System.Byte</rd:TypeName>
        </Field>
        <Field Name=""ReportName"">
          <DataField>ReportName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""PartyCaption"">
          <DataField>PartyCaption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""PartySuffix"">
          <DataField>PartySuffix</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""ReportTitle"">
          <DataField>ReportTitle</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""PartyAddress"">
          <DataField>PartyAddress</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""SubReportProcList"">
          <DataField>SubReportProcList</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""PartyStateName"">
          <DataField>PartyStateName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""PartyStateCode"">
          <DataField>PartyStateCode</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""PartyMobileNo"">
          <DataField>PartyMobileNo</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""CustomerId"">
          <DataField>CustomerId</DataField>
          <rd:TypeName>System.Int32</rd:TypeName>
        </Field>
        <Field Name=""Party_TIN_NO"">
          <DataField>Party TIN NO</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Party_PAN_NO"">
          <DataField>Party PAN NO</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Party_AADHAR_NO"">
          <DataField>Party AADHAR NO</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Party_GST_NO"">
          <DataField>Party GST NO</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Party_CST_NO"">
          <DataField>Party CST NO</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""SignatoryMiddleCaption"">
          <DataField>SignatoryMiddleCaption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""SignatoryRightCaption"">
          <DataField>SignatoryRightCaption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""ProductCaption"">
          <DataField>ProductCaption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Rate"">
          <DataField>Rate</DataField>
          <rd:TypeName>System.Decimal</rd:TypeName>
        </Field>
        <Field Name=""Amount"">
          <DataField>Amount</DataField>
          <rd:TypeName>System.Decimal</rd:TypeName>
        </Field>
        <Field Name=""Dimension1Name"">
          <DataField>Dimension1Name</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Dimension1Caption"">
          <DataField>Dimension1Caption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Dimension2Name"">
          <DataField>Dimension2Name</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Dimension2Caption"">
          <DataField>Dimension2Caption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Dimension3Name"">
          <DataField>Dimension3Name</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Dimension3Caption"">
          <DataField>Dimension3Caption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Dimension4Name"">
          <DataField>Dimension4Name</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Dimension4Caption"">
          <DataField>Dimension4Caption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Specification"">
          <DataField>Specification</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""SpecificationCaption"">
          <DataField>SpecificationCaption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""SignatoryleftCaption"">
          <DataField>SignatoryleftCaption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""LineRemark"">
          <DataField>LineRemark</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""SalesTaxProductCodes"">
          <DataField>SalesTaxProductCodes</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""SalesTaxProductCodeCaption"">
          <DataField>SalesTaxProductCodeCaption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""ProductGroupName"">
          <DataField>ProductGroupName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""ProductGroupCaption"">
          <DataField>ProductGroupCaption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""ProductUidName"">
          <DataField>ProductUidName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""ProductUidCaption"">
          <DataField>ProductUidCaption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""CostCenterName"">
          <DataField>CostCenterName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""CostCenterCaption"">
          <DataField>CostCenterCaption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""CostCenterCnt"">
          <DataField>CostCenterCnt</DataField>
          <rd:TypeName>System.Int32</rd:TypeName>
        </Field>
        <Field Name=""SalesTaxGroupProductCaption"">
          <DataField>SalesTaxGroupProductCaption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
      </Fields>
      <rd:DataSetInfo>
        <rd:DataSetName>Purchase</rd:DataSetName>
        <rd:SchemaPath>E:\Satyam\SuryaIndia\SuryaIndia\SuppliorModule_V1\Source\Surya.Planning.Reports\Reports\Purchase.xsd</rd:SchemaPath>
        <rd:TableName>ProcPurchaseInvoicePrint</rd:TableName>
        <rd:TableAdapterFillMethod>Fill</rd:TableAdapterFillMethod>
        <rd:TableAdapterGetDataMethod>GetData</rd:TableAdapterGetDataMethod>
        <rd:TableAdapterName>ProcPurchaseInvoicePrintTableAdapter</rd:TableAdapterName>
      </rd:DataSetInfo>
    </DataSet>
    <DataSet Name=""DsGatePass"">
      <Query>
        <DataSourceName>Purchase</DataSourceName>
        <CommandText />
      </Query>
      <Fields>
        <Field Name=""GatePassHeaderId"">
          <DataField>GatePassHeaderId</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""DocNo"">
          <DataField>DocNo</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""DocDate"">
          <DataField>DocDate</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""Remark"">
          <DataField>Remark</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""PersonName"">
          <DataField>PersonName</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""GodownName"">
          <DataField>GodownName</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""GatePassLineId"">
          <DataField>GatePassLineId</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""Product"">
          <DataField>Product</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""Specification"">
          <DataField>Specification</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""Qty"">
          <DataField>Qty</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""UnitName"">
          <DataField>UnitName</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""DecimalPlaces"">
          <DataField>DecimalPlaces</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""ReferenceDocNo"">
          <DataField>ReferenceDocNo</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""ReportTitle"">
          <DataField>ReportTitle</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""SignatoryleftCaption"">
          <DataField>SignatoryleftCaption</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""SignatoryMiddleCaption"">
          <DataField>SignatoryMiddleCaption</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""SignatoryRightCaption"">
          <DataField>SignatoryRightCaption</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
      </Fields>
    </DataSet>
  </DataSets>
  <ReportParameters>
    <ReportParameter Name=""ReportTitle"">
      <DataType>String</DataType>
      <Nullable>true</Nullable>
      <AllowBlank>true</AllowBlank>
      <Prompt>ReportParameter1</Prompt>
    </ReportParameter>
    <ReportParameter Name=""ReportSubtitle"">
      <DataType>String</DataType>
      <Nullable>true</Nullable>
      <AllowBlank>true</AllowBlank>
      <Prompt>ReportParameter1</Prompt>
    </ReportParameter>
    <ReportParameter Name=""CompanyName"">
      <DataType>String</DataType>
      <Nullable>true</Nullable>
      <AllowBlank>true</AllowBlank>
      <Prompt>ReportParameter1</Prompt>
    </ReportParameter>
    <ReportParameter Name=""Id"">
      <DataType>Integer</DataType>
      <Nullable>true</Nullable>
      <Prompt>Id</Prompt>
    </ReportParameter>
  </ReportParameters>
  <Code>
    Function RupeesToWord(ByVal MyNumber)
    Dim Temp
    Dim Rupees, Paisa As String
    Dim DecimalPlace, iCount
    Dim Hundreds, Words As String
    Dim place(9) As String
    place(0) = "" Thousand ""
    place(2) = "" Lakh ""
    place(4) = "" Crore ""
    place(6) = "" Arab ""
    place(8) = "" Kharab ""
    On Error Resume Next
    ' Convert MyNumber to a string, trimming extra spaces.
    MyNumber = Trim(Str(MyNumber))

    ' Find decimal place.
    DecimalPlace = InStr(MyNumber, ""."") 

    ' If we find decimal place...
    If DecimalPlace &gt; 0 Then
    ' Convert Paisa
    Temp = Left(Mid(MyNumber, DecimalPlace + 1) &amp; ""00"", 2)
    'Paisa = "" and "" &amp; ConvertTens(Temp) &amp; "" Paisa""
    Paisa = """"

    ' Strip off paisa from remainder to convert.
    MyNumber = Trim(Left(MyNumber, DecimalPlace - 1))
    End If

    '===============================================================
    Dim TM As String ' If MyNumber between Rs.1 To 99 Only.
    TM = Right(MyNumber, 2)

    If Len(MyNumber) &gt; 0 And Len(MyNumber) &lt;= 2 Then
    If Len(TM) = 1 Then
    Words = ConvertDigit(TM)
    RupeesToWord = Words &amp; Paisa

    Exit Function

    Else
    If Len(TM) = 2 Then
    Words = ConvertTens(TM)
    RupeesToWord = Words &amp; Paisa
    Exit Function

    End If
    End If
    End If
    '===============================================================


    ' Convert last 3 digits of MyNumber to ruppees in word.
    Hundreds = ConvertHundreds(Right(MyNumber, 3))
    ' Strip off last three digits
    MyNumber = Left(MyNumber, Len(MyNumber) - 3)

    iCount = 0
    Do While MyNumber &lt;&gt; """"
    'Strip last two digits
    Temp = Right(MyNumber, 2)
    If Len(MyNumber) = 1 Then


    If Trim(Words) = ""Thousand"" Or _
    Trim(Words) = ""Lakh Thousand"" Or _
    Trim(Words) = ""Lakh"" Or _
    Trim(Words) = ""Crore"" Or _
    Trim(Words) = ""Crore Lakh Thousand"" Or _
    Trim(Words) = ""Arab Crore Lakh Thousand"" Or _
    Trim(Words) = ""Arab"" Or _
    Trim(Words) = ""Kharab Arab Crore Lakh Thousand"" Or _
    Trim(Words) = ""Kharab"" Then

    Words = ConvertDigit(Temp) &amp; place(iCount)
    MyNumber = Left(MyNumber, Len(MyNumber) - 1)

    Else

    Words = ConvertDigit(Temp) &amp; place(iCount) &amp; Words
    MyNumber = Left(MyNumber, Len(MyNumber) - 1)

    End If
    Else

    If Trim(Words) = ""Thousand"" Or _
    Trim(Words) = ""Lakh Thousand"" Or _
    Trim(Words) = ""Lakh"" Or _
    Trim(Words) = ""Crore"" Or _
    Trim(Words) = ""Crore Lakh Thousand"" Or _
    Trim(Words) = ""Arab Crore Lakh Thousand"" Or _
    Trim(Words) = ""Arab"" Then


    Words = ConvertTens(Temp) &amp; place(iCount)


    MyNumber = Left(MyNumber, Len(MyNumber) - 2)
    Else

    '=================================================================
    ' if only Lakh, Crore, Arab, Kharab

    If Trim(ConvertTens(Temp) &amp; place(iCount)) = ""Lakh"" Or _
    Trim(ConvertTens(Temp) &amp; place(iCount)) = ""Crore"" Or _
    Trim(ConvertTens(Temp) &amp; place(iCount)) = ""Arab"" Then

    Words = Words
    MyNumber = Left(MyNumber, Len(MyNumber) - 2)
    Else
    Words = ConvertTens(Temp) &amp; place(iCount) &amp; Words
    MyNumber = Left(MyNumber, Len(MyNumber) - 2)
    End If

    End If
    End If

    iCount = iCount + 2
    Loop

    RupeesToWord = Words &amp; Hundreds &amp; Paisa
    End Function

    ' Conversion for hundreds
    '*****************************************
    Private Function ConvertHundreds(ByVal MyNumber)
    Dim Result As String

    ' Exit if there is nothing to convert.
    If Val(MyNumber) = 0 Then Exit Function

    ' Append leading zeros to number.
    MyNumber = Right(""000"" &amp; MyNumber, 3)

    ' Do we have a hundreds place digit to convert?
    If Left(MyNumber, 1) &lt;&gt; ""0"" Then
    Result = ConvertDigit(Left(MyNumber, 1)) &amp; "" Hundred ""
    End If

    ' Do we have a tens place digit to convert?
    If Mid(MyNumber, 2, 1) &lt;&gt; ""0"" Then
    Result = Result &amp; ConvertTens(Mid(MyNumber, 2))
    Else
    ' If not, then convert the ones place digit.
    Result = Result &amp; ConvertDigit(Mid(MyNumber, 3))
    End If

    ConvertHundreds = Trim(Result)
    End Function

    ' Conversion for tens
    '*****************************************
    Private Function ConvertTens(ByVal MyTens)
    Dim Result As String

    ' Is value between 10 and 19?
    If Val(Left(MyTens, 1)) = 1 Then
    Select Case Val(MyTens)
    Case 10 : Result = ""Ten""
    Case 11 : Result = ""Eleven""
    Case 12 : Result = ""Twelve""
    Case 13 : Result = ""Thirteen""
    Case 14 : Result = ""Fourteen""
    Case 15 : Result = ""Fifteen""
    Case 16 : Result = ""Sixteen""
    Case 17 : Result = ""Seventeen""
    Case 18 : Result = ""Eighteen""
    Case 19 : Result = ""Nineteen""
    Case Else
    End Select
    Else
    ' .. otherwise it's between 20 and 99.
    Select Case Val(Left(MyTens, 1))
    Case 2 : Result = ""Twenty ""
    Case 3 : Result = ""Thirty ""
    Case 4 : Result = ""Forty ""
    Case 5 : Result = ""Fifty ""
    Case 6 : Result = ""Sixty ""
    Case 7 : Result = ""Seventy ""
    Case 8 : Result = ""Eighty ""
    Case 9 : Result = ""Ninety ""
    Case Else
    End Select

    ' Convert ones place digit.
    Result = Result &amp; ConvertDigit(Right(MyTens, 1))
    End If

    ConvertTens = Result
    End Function

    Private Function ConvertDigit(ByVal MyDigit)
    Select Case Val(MyDigit)
    Case 1 : ConvertDigit = ""One""
    Case 2 : ConvertDigit = ""Two""
    Case 3 : ConvertDigit = ""Three""
    Case 4 : ConvertDigit = ""Four""
    Case 5 : ConvertDigit = ""Five""
    Case 6 : ConvertDigit = ""Six""
    Case 7 : ConvertDigit = ""Seven""
    Case 8 : ConvertDigit = ""Eight""
    Case 9 : ConvertDigit = ""Nine""
    Case Else : ConvertDigit = """"
    End Select
    End Function



    Public Function GenerateVbCrLf(ByVal Count as integer)
    dim Value as String
    dim i as integer
    For i = 1 To Count Step 1
    Value = Value &amp; "" "" &amp; VbCrLf
    Next i
    Return Value
    End Function

Public Function PrintProduct(ProductName as String,ProductGroupCaption as String,ProductGroupName as String,SpecificationCaption as String,Specification as String,Dimension1Caption as String,Dimension1Name as String,Dimension2Caption as String,Dimension2Name as String,Dimension3Caption as String,Dimension3Name as String,Dimension4Caption as String,Dimension4Name as String,ProductUidCaption as String,ProductUidName as String)
dim str as String
Dim C As Integer = 0
  str =iif(ProductName  &lt;&gt; """",ProductName+""&lt;BR&gt;""  ,"""")
  if ProductGroupName &lt;&gt; """"  Then 
     str =str +""&lt;b&gt;""+ProductGroupCaption+""&lt;/b&gt;""+"" : ""+ ProductGroupName
  C=C+1
 End If


if Specification &lt;&gt; """"  Then
str =str +"" ""+iif(C mod 2=1,"" , "","""")    
  C=C+1
     str =str +""&lt;Br&gt;""+""&lt;b&gt;""+SpecificationCaption+""&lt;/b&gt;"" +""  : ""+ Specification+iif(C mod 2=0,""&lt;BR&gt;"","""")  
 End If

  if Dimension1Name &lt;&gt; """"  Then
    str =str +"" ""+iif(C mod 2=1,"" , "","""")    
     C=C+1
     str =str +"" ""+""&lt;b&gt;""+Dimension1Caption+""&lt;/b&gt;"" + "" : ""+ Dimension1Name +iif(C mod 2=0,""&lt;BR&gt;"","""")    
 End If      

  if Dimension2Name &lt;&gt; """"  Then
   str =str +"" ""+iif(C mod 2=1,"" , "","""")    
     C=C+1
     str =str +""&lt;b&gt;""+Dimension2Caption+""&lt;/b&gt;"" +"" : ""+ Dimension2Name +iif(C mod 2=0,""&lt;BR&gt;"","""")    
 End If   

  if Dimension3Name &lt;&gt; """"  Then
  str =str +"" ""+iif(C mod 2=1,"" , "","""")    
     C=C+1
     str =str +""&lt;b&gt;""+Dimension3Caption+""&lt;/b&gt;"" +"" : ""+ Dimension3Name +iif(C mod 2=0,""&lt;BR&gt;"","""")    
 End If   

  if Dimension4Name &lt;&gt; """"  Then
   str =str +"" ""+iif(C mod 2=1,"" , "","""")    
     C=C+1
     str =str + ""&lt;b&gt;""+Dimension4Caption+""&lt;/b&gt;"" +"" : ""+ Dimension4Name +iif(C mod 2=0,""&lt;BR&gt;"","""")    
 End If  

  if ProductUidName &lt;&gt; """"  Then
    str =str +"" ""+iif(C mod 2=1,"" , "","""")    
     C=C+1
       str =str +"" ""+""&lt;b&gt;""+ProductUidCaption +""&lt;/b&gt;""+"" : ""+ ProductUidName+iif(C mod 2=0,""&lt;BR&gt;"","""")    
 End If   
 
if Right(str, 4).toupper()=""&lt;BR&gt;"" Then
	str =str.Substring(0,str.Length-4)
End If
return str 
End Function



Public Function PrintLineRemark(Remark as String,Lot as String,DiscountPer as String,DiscountAmt as String)
dim str as String
Dim C As Integer = 0
  str =iif(Remark &lt;&gt; """",Remark ,"""")

  if Lot &lt;&gt; """"  Then 
  'str =str +""&lt;BR&gt;""+""&lt;b&gt;""+""Lot""+""&lt;/b&gt;""+"" : ""+ Lot 
   str =str +"" Lot : ""+ Lot 
     C=C+1 
 End If

if DiscountPer &lt;&gt; """"  Then
    str =str +"" ""+iif(C mod 2=1,"" , "","""")    
     C=C+1
       str =str +"" ""+""&lt;b&gt;""+""Dis Per"" +""&lt;/b&gt;""+"" : ""+ DiscountPer +iif(C mod 2=0,""&lt;BR&gt;"","""")    
 End If  

if DiscountAmt &lt;&gt; """"  Then
    str =str +"" ""+iif(C mod 2=1,"" , "","""")    
     C=C+1
       str =str +"" ""+""&lt;b&gt;""+""Dis Amt"" +""&lt;/b&gt;""+"" : ""+ DiscountAmt +iif(C mod 2=0,""&lt;BR&gt;"","""")    
 End If

if Right(str, 4).toupper()=""&lt;BR&gt;"" Then
	str =str.Substring(0,str.Length-4)
End If

return str 
End Function

</Code>
  <EmbeddedImages>
    <EmbeddedImage Name=""SuryaLogo"">
      <MIMEType>image/jpeg</MIMEType>
      <ImageData>/9j/4AAQSkZJRgABAQAAAQABAAD/2wCEAAkGBwgHBgkIBwgKCgkLDRYPDQwMDRsUFRAWIB0iIiAdHx8kKDQsJCYxJx8fLT0tMTU3Ojo6Iys/RD84QzQ5OjcBCgoKDQwNGg8PGjclHyU3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3N//AABEIAGEAYAMBEQACEQEDEQH/xAAbAAACAwEBAQAAAAAAAAAAAAADBAABBQIGB//EADkQAAEDAwIDBgQDBgcAAAAAAAECAwQABRESITFBUQYTFGFxgRUikaEysfAjM0JSc4IkNDVjcsHx/8QAGQEAAwEBAQAAAAAAAAAAAAAAAAIDBAEF/8QAKhEAAgICAQQBAwQDAQAAAAAAAQIAAxESBBMhMUEiUXGBFDNhoSMywbH/2gAMAwEAAhEDEQA/APuNEJKISlKCeJArhOIScq7CIQU3AS5RmraVHKh3AQNwPP7VCsW7tvjHqVc16jTz7l3oR1WyQmXI8MwU4U9qxp96pZV1VKfWTW0VMHPqEtXc/Do4iv8AiGQgBDurVqHXNCV9NQn0gbBYdx7gpSLgq4xlRnW0xAD3yFD5j0xU3FpsBU/H3HQ16HI7+o/wFXk5SFpWkKQoKB5g5rgIPiE6rsJKISUQlZohErrbWbpGDEgrCAsL+RWDkVG6lbl1aUqtaptlilj7RQbw/JjQ+8Co2x1pwFDOMj6VoNZQCZKuQlpIX1FblZXXO0kW7m5lhhkAKZJwDjOwOcb53oNqIh2nG47PcHB8eo/fo8Cba3GLkoiM5jJScHIORipNetI3Y9pZqesNCJ3ZWoMO1NM29f8AhWwcKUd+OTnzzXF5CWjqA9p1aekOmB4mbZrGuLfpl1FyMhmTnS2NwMnO5zjbgKt1VdAFkEoau1nJ8w987QwbbNjW+Wl0rljGUDZIJ07+/SgVF1M7ZyVqcKfJmha7exbIojRQoNgk
/Mcnes9FK0povia7bWtbZvMcq0nJRCUaITPkwSu5MzvEvJDKCCyk/Kr2/XAVBqtrQ+3j1KC0LWUx59xPs12jj9oUSTHZda7hQB7zG4OcHb0O1anQpiY6OQt2cDxObk41ZVg26Cwl6UoqcUlvGs+eOJ3rzudzHp1CjJM2cfj1sSfENLgPXdiKt5RjKTkrbxnj+vvU7uM/LRC/x/iOlgqJx3jHweMqC1EdLi0NHKSVYP29at+jraoVtkgReqwYsJSrPGEFcNsuIbWrUSDk59/Sg8OsVGpSQDDrNtsYvFgv2iJJMYmStRBQgjGPv+sVKrjvxKm0+X0jPYLWGe05tLqLsouT4bJfjL+Ram90k9M7g7U/B5dlysGGCIl9KAg+Zx2m7St9n1xEORXX/EKI+Q40gY+p34V6KJsD3mO/kCkgYzmPxLcli4yZoddUqQBlCj8qcdKypTpYz5PebGtLIFx4mhV5OIXCZJjSIrbENb6Hl6XFpOzY23P3+lQttdGUKuc/1K1orAknGJSbzAVdFWwSUGYkZLW+evHhnG+K06HG3qZuqm+me8FeGXWYCxbGwhal5X3ScE54nbnwrDzjaav8XmaKBWH+UPa4zzURtMxZdeG+VblPlmn41TpWBYcmFjAsdR2j1aZOSiElEJWKIRG7RXZERaIjhaczq+XbV5E1l5VTvWRWcGUrYK2Wg7O0+YLYuCcuJUSjvBkpHL3rnCFopAt8wu0LfHxIb5bxePhPiB4zGdGD0zjPDON8Vt0bG3qZusm/Tz3hLbImvuyRMi9whDmGjqzrT1/XWs1L2MW3XH0mqxUUDQ5jxNXkpnIsluRdlXVLA8YoYLmo9MZxwzjbNMXOuskKUD9THeJ2mJNTc5D8xTmBkJyrZWTy8hXkcSm4XM9pP/JttdNAFE3civVmeXRCVmiEmaISZohJmiEw7rAluXViREUvBxqOrZGP+scq8vlce5uQrVmaK7FFZVo98It5uYuZjI8ZjHe8+GPT
ONs16uxxiY+km++O8eyKWUmP8Jkx7ZPYiT3vESNRbdcP7skbYqXHq6OcknJjchzauFGDiddmYU+32pDF1leJkBROvUVYHIZO5rS5DHImehHRMOcmJQo0S6SZyLitapyH1gN98pKmm8kIKACMApwcjiSa6SVAx4k1VXYhj3+8dfSWbtaWkuLUkNuglSslWEp3PU0vkGUIw6j7zUe/dL/4n8qWVPieXhNJmRezbUkrWhcIqWNahqIQjc4O/GqnttiZUGy15+kcbjNx7wq3MLdVEfirW8yp1Su6OQAQScp1Aq2z/DtS57Zj4xZoPBE6iXE26PIiXNxSnIaQpDhGVPtHZJ81Z+Ujr6igjPiCvoCrev7hGVu262yrlcNSpDiS6toHIR/K2n7DzJNcPc4E6CUQu3mB7Ph+DIdt01wrdWgSkqJ4lX7wD0Xv6LFdbuMzlOVJU/eH7T26XdLWqNBlmM6VpVqyRkDlkb0IwU5Ma9GsTVTgyvgpkW6BGmy3nHYukqdSrBcUBxNZ+RStxBPbBzNHGdqUx57YhHL5D+HzZjClPIhhRcSlJByOQzXabUuPwM5erUrsw/mV2cvKL7bBNQypkaykpUc8Oh5irumpxIU3C1NsYnU1i2XOCJbi21NJSVtykLwWx/MlY4VwEgwcI67TNZlOJb7PzrgrTqQpDjihgBS0jST0zj6mm+oEmCRozTbuUtmHAekPrCW0oJyee2wHUnpSAd5d2CrkzBbt6VJ7OwpqFZaiKSpIUU4UEIHEVTP+xEgE/bVvpG7a03Z7i5AKcNSlFyO6dyo/xIUo8SOIzy9KU/IZjIBW2p9wt5ZbcudlUtAURKVgkf7az+aQfYUKexnbQC6ff/hlXhCp86LbUOLbSn/EurSASAk/INwRurf+00L2GYWjdgv5i90ivQCxdVzZEgRF5cS4EAd0rAXwSOAwr+2gHPbEWxSmLM5xG+0l4+B2pU4MF/CgnSDgDPMnkK4i7NiNfb0k2AzKTf4y
LbCmyUOMiWE6UFOSkkc6hfclBAb2cTTx0a9cqPWZpNR2WQoNNIQFnUrSkDJ6mmVVXwMThJPmcNvRW3vCNuMpdCdQZSQCB1xT98ZiZUHUTztuats28SEybbFS4VFSP2YyVA/xDgT515/G572XNU34lbOJWqhwJ6d1lt5tTbraVtqGFIUMgj0r0MyZAIwYlGsdrjOpdYgMIWj8BCPwenT2ptjEFKDwI4phpTzbqkJLjeQhRG6c8cfSlzHx3zJIjMyAgPNpWELC06h+FQ4EedGYFQfMjjDTi21uNpUppWpskfhOCMj2JHvXcwIBkSw0h9b4bSHVgBS8bkDOB9zXIYGczGvdyU1NahtoS6lYw82oZ1BW2PpmvN5XMeq9UQfeaEpD1ktNNEmC0+i3h9lL4QNLGsatI8vavTwfMy7KDpmMKQheNSQrByMjgaUgHzHH8RO5G4h2N8PDRQXP2/eck+X3qNxtyvT/ADKV9PB3/ETHZuIO0RvfeO+Ix+DI0506c9eFauoddZj/AE69Xq+4xeXDCiLlRmUd9kAr07gda8/mOaazYg7zbUN21Y9oa1ThPiB3SUqB0qGNs+VPxb+vWGnLE0bEcrVJy6ISUQkohErrN8FDU8EFR4JGNs+flWfk39GstiPWuzYgLI87MiJfloT3oUQlenBI6/nUuE7XV72DvGuUI2FME52dgrvyL0e88SkcAr5ScYzj0r0NzrrMZ46Gzqe41bkXBEiUZzjSmSv9gEDcJ8/tWWoWhm3Pb1NdhrwNPzHsVeTicm5Ro8xiG44Uvv57saSfvUWvRXCHyY4qdkLDwJl9mLLcLUuabjcDMS8sFAJJxxyTngTngOlabGVgMCY+PVZWWLNnMZvjExxDJt6iC0okpSrHpXmc2q4qpp9TfS6ZO86Xcl2+LHNySS85kK7sZx+tq63KNFa9Ydz9ICrdjp4jSLnEXFTJ74JaUdIUoY36VdeVS1Ys27GIa2219yKuUQRlyUvBbSDhRQM4
PtQeVUENmewh02zjEWZuZuLElNvBS6gDSXQMHP8A5UE5f6lWFPkR2q6ZG8HZI8xgvruK93SMJUsHJ61zg03qGN3uFzocawHaqxS70mIIc9UTuHNSgM/Nw32PEcvWvTrcL5EwcilrcatjE0Y1yYdnu25KlmQwkFeU7Hhz9xWZb0aw1jyJtNLrWHPgx+rSclEINTLSnEuKbQVo/CojcehrhUE5InQSBgQcyWxCZL0p1LTYIGpVLZYta7McCdRGc4UZmL2f7PG2XOdcPHLkIlnUEkcATnJOd+lWNgdRiZKuOanYk+ZL5erazdolnmxnHVyCkpUBsgqOBzzx6VJ+Kt6fIZAjtyhVYE9mP3NiA1a1JllLEVoaioHGjzqL8Wu2vpY7TR1jWeoTKs7Nudtg8EtMiM7klZ31ct/pRXxEpQ14nOv1fmDELRercb7JskOK404zkqXj5VEYz58+dXTjLSmUGAZAcsW2ms+RO+0fZtN6lwpK5jjHhFasJGc7g5HQ7caotmgMW7j9Vgc4xNaBNjT2S7EdDiAopJHWoV2paNkOZretqzhhGA2gKKwkBR4nG5p8DOYk6rsJKISUQgZcViY0WpLSXWyQSlQyKR0VxhhkRlZlOVhEpCEBKQAAMADlTDsIvmZjKzKu7yJFtCRGA7mSsA5zx0nG3tUq7XNjIRgD3HepNVfOTD3lmG/bJDdyTmLoy5x4DflVTYKxufUmaur8D7g7AxBYtMdFrSRFwSjVnPHfOeea4louUWDwYCjof4/pOZi3ItxjmLbg6ZB0vPpABQBwyf1wqdtrqyqBkH+pSupCGYnB/wDZpAZG9ViQcaMxFR3cZlDSMk6UJwMmuKiqMKMRmZmOWMNTRZKISUQkohJRCSiE4HEV2cgbl/kZH9M/lUb/ANtvtKV/7iCsv+lxv6YpeL+yv2jX/uNG1cavIGdDhROiXROyUQkohP/Z</ImageData>
    </EmbeddedImage>
  </EmbeddedImages>
  <rd:ReportUnitType>Inch</rd:ReportUnitType>
  <rd:ReportID>f1b70379-da78-4ea9-bab5-cbb93d10df58</rd:ReportID>
</Report>";

            return StringCode;

        }
    }
}