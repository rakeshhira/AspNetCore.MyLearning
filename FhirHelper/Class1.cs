using System;
using Hl7.Fhir.Model;

namespace FhirResourceSamples
{
	public static class PatientSample
	{
		public static Patient Create(
			string ssn = null,
			string firstName = null, string middleName = null, string familyName = null, string namePrefix = null, string nickName = null,
			AdministrativeGender gender = AdministrativeGender.Male, string birthDate = null, DateTime? birthTime = null, string birthCity = null,
			string addressLine = null, string addressCity = null, string addressState = null, string addressPostalCode = null, string addressCountry = null,
			string contactNameGiven = null, string contactNameFamily = null, AdministrativeGender contactGender = AdministrativeGender.Male, string contactRelationshipCode = null, bool deceased = false)
		{
			var patient = new Patient();

			var id = new Identifier();
			id.System = "http://hl7.org/fhir/sid/us-ssn";
			id.Value = ssn ?? "000-12-3456";
			patient.Identifier.Add(id);

			var name = new HumanName().WithGiven(firstName ?? "Christopher").WithGiven(middleName ?? "C.H.").AndFamily(familyName ?? "Parks");
			name.Prefix = new string[] { namePrefix ?? "Mr." };
			name.Use = HumanName.NameUse.Official;

			var nickname = new HumanName();
			nickname.Use = HumanName.NameUse.Nickname;
			nickname.GivenElement.Add(new FhirString(nickName ?? "Chris"));

			patient.Gender = gender; // AdministrativeGender.Male;

			patient.BirthDate = birthDate ?? "1983-04-23";

			var birthplace = new Extension();
			birthplace.Url = "http://hl7.org/fhir/StructureDefinition/birthPlace";
			birthplace.Value = new Address() { City = birthCity ?? "Seattle" };
			patient.Extension.Add(birthplace);

			var birthtime = new Extension("http://hl7.org/fhir/StructureDefinition/patient-birthTime",
										   birthTime != null ? new FhirDateTime(birthTime.Value) : new FhirDateTime(1983, 4, 23, 7, 44));
			patient.BirthDateElement.Extension.Add(birthtime);

			var address = new Address()
			{
				Line = new string[] { addressLine ?? "3300 Washtenaw Avenue, Suite 227" },
				City = addressCity ?? "Ann Arbor",
				State = addressState ?? "MI",
				PostalCode = addressPostalCode ?? "48104",
				Country = addressCountry ?? "USA"
			};

			var contact = new Patient.ContactComponent();
			contact.Name = new HumanName();
			contact.Name.Given = new string[] { contactNameGiven ?? "Susan" };
			contact.Name.Family = contactNameFamily ?? "Parks";
			contact.Gender = contactGender; // AdministrativeGender.Female;
			contact.Relationship.Add(new CodeableConcept("http://hl7.org/fhir/v2/0131", contactRelationshipCode ?? "N"));
			contact.Telecom.Add(new ContactPoint(ContactPoint.ContactPointSystem.Phone, null, ""));
			patient.Contact.Add(contact);

			patient.Deceased = new FhirBoolean(deceased);

			return patient;
		}
	}
}
