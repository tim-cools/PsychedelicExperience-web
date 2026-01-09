namespace PsychedelicExperience.Business

    open System

    module UserProfile = 
        exception ItemAlreadyExists of string
        exception ItemNotFound of string
        exception DomainError of string

        type Id = Guid
        type Name = String
        type UserName = Name
        type FullName = Name
    
        type UserProfile = { 
            Id: Id
            UserName: UserName
            FullName: FullName
        }
        
        type  Command = 
            | CreateUserProfile       of Id: Id * Name: Name
            | SetUserProfileUserName  of Id: Id * UserName: UserName
            | SetUserProfileFullName  of Id: Id * FullName: FullName
         
        type Event = 
            | UserProfileCreated       of Id: Id * Name: Name
            | UserProfileUserNameSet   of Id: Id * UserName: UserName
            | UserProfileFullNameSet   of Id: Id * FullName: FullName

        type Query = 
            | ById of Id * Name
            | List of Id * UserName


        let apply profile event = 
            match profile, event with
                | None, UserProfileCreated(id, name) -> 
                    { Id = id; FullName = name; UserName = name }
                
                | None, _-> 
                    raise (System.ArgumentException("Unknown event: " + event.GetType().FullName))
                
                | Some(profile), UserProfileUserNameSet(id, name) -> 
                    { profile with UserName = name; }
                
                | Some(profile), UserProfileFullNameSet(id, name) -> 
                    { profile with FullName =  name; }
                
                | Some(profile), _-> 
                    raise (System.ArgumentException("Unknown event: " + event.GetType().FullName))

        let handle events command = 
            let item = events |> Seq.fold (fun acc e -> Some(apply acc e)) None
 
            match item, command with
                | None,          CreateUserProfile(id, name) -> 
                    [UserProfileCreated(id, name)]

                | Some(profile), CreateUserProfile(id, name) -> 
                    raise (ItemAlreadyExists("profile already created"))

                | None, _ -> 
                    raise (ItemNotFound("profile not found"))
                
                | Some(profile), SetUserProfileUserName(id, name) -> 
                    [UserProfileUserNameSet(id, name)]

                | Some(profile), SetUserProfileFullName(id, name) -> 
                    [UserProfileFullNameSet(id, name)]

                | Some(profile), _ -> 
                    raise (ItemNotFound("profile not found"))