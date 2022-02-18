<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
xmlns:msxsl="urn:schemas-microsoft-com:xslt"
xmlns:umbraco.library="urn:umbraco.library"
xmlns:user="urn:my-scripts">

	<xsl:output method="text" indent="no" encoding="utf-8" />

	<xsl:param name="records" />
  <xsl:variable name="firstRecord" select="$records//uformrecord[1]" />
  <xsl:variable name="fieldNames" select="$firstRecord/fields" />
  
	<xsl:template match="/">
		<xsl:text>"State","Submitted","PageId","URL","IP","MemberId",</xsl:text>
		<xsl:for-each select="$fieldNames/*">
			<xsl:sort select="@pageindex" data-type="number" order="ascending" />
			<xsl:sort select="@fieldsetindex" data-type="number" order="ascending" />
			<xsl:sort select="@sortorder" data-type="number" order="ascending" />
			<xsl:text>"</xsl:text><xsl:value-of select="normalize-space(caption)"/><xsl:text>"</xsl:text>
			<xsl:if test="position() != last()">
				<xsl:text>,</xsl:text>
			</xsl:if>
		</xsl:for-each>
		<xsl:text>&#xD;</xsl:text>
		<xsl:for-each select="$records//uformrecord">
			<xsl:variable name="record" select="." />
			<xsl:text>"</xsl:text><xsl:value-of select="state"/>","<xsl:value-of select="substring(updated,9,2)"/>/<xsl:value-of select="substring(updated,6,2)"/>/<xsl:value-of select="substring(updated,1,4)"/>","<xsl:value-of select="pageid"/>","<xsl:value-of select="umbraco.library:NiceUrl(pageid)"/>","<xsl:value-of select="ip"/>","<xsl:value-of select="memberkey"/><xsl:text>",</xsl:text>
			<xsl:for-each select="$fieldNames/*">
				<xsl:sort select="@pageindex" data-type="number" order="ascending" />
				<xsl:sort select="@fieldsetindex" data-type="number" order="ascending" />
				<xsl:sort select="@sortorder" data-type="number" order="ascending" />
				<xsl:variable name="matches" select="." />
				<xsl:variable name="values" select="$record/fields/*[./caption/text() = $matches/caption]/values" />
				<xsl:if test="$values">
<!--					<xsl:choose>
						<xsl:when test="count($values//value) &gt; 1">
							<xsl:text>"</xsl:text><xsl:for-each select="$values//value">
								<xsl:copy-of select="normalize-space(translate(translate(.,'£',''),',',''))"/>
								<xsl:if test="position() != last()">
									<xsl:text>;</xsl:text>
								</xsl:if>
							</xsl:for-each>
							<xsl:text>"</xsl:text>
						</xsl:when>
						<xsl:otherwise>-->
							<xsl:text>"</xsl:text><xsl:value-of select="normalize-space(translate(translate($values//value,'£',''),',',''))"/><xsl:text>"</xsl:text>
						<!--</xsl:otherwise>
					</xsl:choose>-->
				</xsl:if>
				<xsl:if test="position() != last()">
					<xsl:text>,</xsl:text>
				</xsl:if>
			</xsl:for-each>
			<xsl:text>&#xD;</xsl:text>
		</xsl:for-each>
	</xsl:template>

</xsl:stylesheet>