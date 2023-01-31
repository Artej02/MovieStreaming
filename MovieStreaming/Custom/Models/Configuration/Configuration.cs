namespace MovieStreaming.Custom.Models.Configuration
{
    public enum CRUDOperation
    {
        Create = 1,
        Update = 2,
        Delete = 3
    }

    public enum AuthorizationType
    {
        ReadWrite = 1,
        Read = 2,
        Block = 3
    }

    public enum RoleAccess
    {
        FullAccess = 1,
        Read = 2,
        Denied = 3
    }
    public enum RoleList
    {
        Admin = 1,
        Superadmin = 3,
        Panelist = 5,
        Principal = 6,
        VETDepartment = 7,
        AgjensiaEPunesimit = 8

    }
    public enum QuestionAnswer
    {
        Equal = 1,
        notEuqal = 2,
        GraterThan = 3,
        LessThan = 4,
        GreaterThanOrEqual = 5,
        LessThanOrEqual = 6

    }

    public enum QuestionType
    {
        Single_Choice = 1,
        Multiple_Choice = 2,
        Text = 3,
        Number = 4,
        Decimal = 5,
        Date = 6,
        Time = 7,
        Note = 8,
        Rating_Scale = 9,
        Numeric_Slider = 10,
        DragAndDrop_Rank = 11,
        DropDown_Menu = 12,
        SingleSelect_Matrix = 13,
        MultiSelect_Matrix = 14,
        SlideBySlide_Matrix = 15,
        Photo = 16,
        Video = 17,
        Geo_location = 18,
        Audio = 19,
        Single_Select_Image = 20,
        Multiple_Select_Image = 21,
        Image_Rating  = 22
    }

    public enum EnumIntOrString
    {
        Int = 4,
        String = 5
    }

    public enum ChangeLogTable
    {
        OrganisationType = 1,
        Participants = 2,
        Organisation = 3,
        Question = 4,
        QuestionOptions = 5,
        QuestionSection = 6,
        Role = 7,
        SurveyQuestions = 8,
        Survey = 9,
        ParticipantsAnswer = 10,
        Users = 11,
        RoleAccess = 12,
        ParticipantSurvey = 13,
        RequestQuestion = 14,
        QuestionOptionRequest = 15,
        GroupOptions = 16,
        ViewAuthorization = 17,
        ResearchPanelTypes = 18,
        ResearchProject = 19
    }

    public enum ChangeLogAction
    {
        Inserte = 10,
        Update = 11,
        Delete = 12,
        Login = 13,
        Logout = 14
    }

    public enum Views
    {
        Dashboard = 1,
        Logs = 3,
        Question = 4,
        QuestionSection = 5,
        Survey = 6,
        Organisation = 7,
        Participants = 8,
        ParticipantsRequestAccount = 9,
        Users = 10,
        UsersRequestAccount = 11,
        Role = 12,
        Authorize = 13,
        DynamicReport = 14,
        OrganisationType = 15,
        SurveyResultsParticipant = 16
    }

    public enum ConstantConfiguration
    {
        Integer = 1,
        String = 2,
        Float = 3,
        Bool = 4,
        Decimal = 5,
        Byte = 6,
        Date = 7,
        Time = 8

    }

}
