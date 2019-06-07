For some reason, the project is called AddNewUser. This is incorrect, and I have no idea why it ended up like that.

Anyway, BOOM! It's the deceptively named CreateNewContextItem, which not only allows you to create new items in your right-click desktop context menu, but also allows you to view existing items, re-order them, add and remove items, and edit existing ones. It also doesn't write anything to registry until you tell it to, so that's nice.

You probably need Visual Studio 2015 or 17 to compile it, you can try and use Mono if you really want to, but it isn't tested.

Obviously, this won't do anything on GNU/Linux or macOS.

One known issue: trying to create more than 52 entries and committing to registry will crash before it gets to write any items past the 52nd one, leaving all the others out and dying horribly in the process. This is due to the fact that entries are always ordered by alphanumeric order in the regkey name, and there doesn't seem to be anything that can be done about it.