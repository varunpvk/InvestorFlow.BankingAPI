namespace IF.Domain.ValueObjects
{
    public sealed record Address
    {
        public Address(
            string firstLine,
            string secondLine,
            string street,
            string council,
            string province,
            string postCode)
        {
            FirstLine = firstLine;
            SecondLine = secondLine;
            Street = street;
            Council = council;
            Province = province;
            PostCode = postCode;
        }
        public string FirstLine { get; private set; }
        public string SecondLine { get; private set; }
        public string Street { get; private set; }
        public string Council { get; private set; }
        public string Province { get; private set; }
        public string PostCode { get; set; }
    }
}
